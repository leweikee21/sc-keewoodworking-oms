using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Acquisition.Commands
{
    public class UpdateAcquisitionStatusCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }

    public class UpdateAcquisitionStatusCommandHandler : IRequestHandler<UpdateAcquisitionStatusCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        public UpdateAcquisitionStatusCommandHandler(IAuthenticatedUserService authenticatedUser, IAcquisitionRepositoryAsync acquisitionRepository)
        {
            _authenticatedUser = authenticatedUser;
            _acquisitionRepository = acquisitionRepository;
        }
        public async Task<Response<int>> Handle(UpdateAcquisitionStatusCommand command, CancellationToken cancellationToken)
        {
            var acquisition = await _acquisitionRepository.GetByIdAsync(command.Id);
            if (acquisition == null)
            {
                throw new ApiException($"Acquisition not found.");
            }
            acquisition.Status = command.Status;
            if (command.ReceivedDate != null)
                acquisition.ReceivedDate = DateTime.SpecifyKind(command.ReceivedDate.Value, DateTimeKind.Utc);
            acquisition.LastModifiedBy = _authenticatedUser.UserId;
            acquisition.LastModified = DateTime.UtcNow;
            await _acquisitionRepository.UpdateAsync(acquisition);
            return new Response<int>(acquisition.Id, "Acquisition updated successfully.");
        }
    }
}
