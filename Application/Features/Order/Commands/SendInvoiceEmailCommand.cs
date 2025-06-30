using Application.DTOs.Email;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Commands
{
    public class SendIncoiveEmailCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class SendIncoiveEmailCommandHandler : IRequestHandler<SendIncoiveEmailCommand, Response<int>>
    {
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public SendIncoiveEmailCommandHandler(IOrderRepositoryAsync orderRepository, IEmailService emailService, IUserService userService)
        {
            _orderRepository = orderRepository;
            _emailService = emailService;            
            _userService = userService;
        }

        public async Task<Response<int>> Handle(SendIncoiveEmailCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(command.Id);
            if (order == null)
                throw new ApiException("Order not found.");

            if (string.IsNullOrEmpty(order.filePath))
                throw new ApiException("PDF not found. Generate first.");

            using var httpClient = new HttpClient();
            var pdfBytes = await httpClient.GetByteArrayAsync(order.filePath);

            var users1 = await _userService.GetUsersByRoleAsync("Order");
            var users2 = await _userService.GetUsersByRoleAsync("Admin");
            var emails = users1.Select(x => x.Data.Email)
                                .Concat(users2.Select(x => x.Data.Email))
                                .Concat(new[] { "ly123@example.com" })
                                .ToList();

            var email = new EmailRequest
            {
                To = emails,
                Subject = $"[Kee Woodworking] Invoice for #OPO{order.Id:D6}",
                Body = $@"
                        Dear LY Furniture,<br><br>
                        We are pleased to inform you that Order #OPO{order.Id:D6} has been completed. 
                        We will proceed with delivery as soon as possible.<br><br>
                        Please find the attached invoice for your reference.<br><br>
                        Best regards,<br>
                        Kee Woodworking"
            };
            await _emailService.SendAsync(email, $"INV-OPO{order.Id:D6}.pdf", pdfBytes);

            order.Status = "Invoiced";
            await _orderRepository.UpdateAsync(order);

            return new Response<int>(order.Id, "Email sent successfully");
        }
    }
}
