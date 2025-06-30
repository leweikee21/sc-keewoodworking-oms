using Application.DTOs.Email;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Acquisition.Commands
{
    public class SendAcquisitionEmailCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class SendAcquisitionEmailCommandHandler : IRequestHandler<SendAcquisitionEmailCommand, Response<int>>
    {
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public SendAcquisitionEmailCommandHandler(IAcquisitionRepositoryAsync acquisitionRepository, IEmailService emailService, IUserService userService)
        {
            _acquisitionRepository = acquisitionRepository;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<Response<int>> Handle(SendAcquisitionEmailCommand command, CancellationToken cancellationToken)
        {
            var acquisition = await _acquisitionRepository.GetByIdAsync(command.Id);
            if (acquisition == null)
                throw new ApiException("Acquisition not found.");

            if (string.IsNullOrEmpty(acquisition.filePath))
                throw new ApiException("PDF not found. Generate first.");

            using var httpClient = new HttpClient();
            var pdfBytes = await httpClient.GetByteArrayAsync(acquisition.filePath);


            var users1 = await _userService.GetUsersByRoleAsync("Inventory");
            var users2 = await _userService.GetUsersByRoleAsync("Admin");
            var emails = users1.Select(x => x.Data.Email)
                                .Concat(users2.Select(x => x.Data.Email))
                                .Concat(new[] { acquisition.Supplier.Email })
                                .ToList();

            var email = new EmailRequest
            {
                To = emails,
                Subject = $"New Purchase Order #AC{acquisition.Id: D4}",
                Body = $"Dear {acquisition.Supplier.Name},<br><br>" +
                        $"Please find attached the acquisition order #{acquisition.Id}.<br><br>" +
                        $"Best regards,<br>Kee Woodworking"
            };
            await _emailService.SendAsync(email, $"AC{acquisition.Id:D4}.pdf", pdfBytes);

            acquisition.Status = "Sent";
            await _acquisitionRepository.UpdateAsync(acquisition);

            return new Response<int>(acquisition.Id, "Email sent successfully");
        }
    }
}
