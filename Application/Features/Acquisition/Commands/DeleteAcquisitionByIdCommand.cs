using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Acquisition.Commands
{
    public class DeleteAcquisitionByIdCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteAcquisitionByIdCommandHandler : IRequestHandler<DeleteAcquisitionByIdCommand, Response<int>>
    {
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IAcquisitionItemRepositoryAsync _acquisitionItemsRepository;

        public DeleteAcquisitionByIdCommandHandler(IAcquisitionRepositoryAsync acquisitionRepository, IAcquisitionItemRepositoryAsync acquisitionItemsRepository)
        {
            _acquisitionRepository = acquisitionRepository;
            _acquisitionItemsRepository = acquisitionItemsRepository;
        }

        public async Task<Response<int>> Handle(DeleteAcquisitionByIdCommand command, CancellationToken cancellationToken)
        {
            var acquisition = await _acquisitionRepository.GetByIdAsync(command.Id);

            if (acquisition == null)
                throw new ApiException($"Acquisition not found.");

            if (acquisition.Status == "Sent" || acquisition.Status == "Received")
                throw new InvalidOperationException($"Cannot delete a acquisiton with status {acquisition.Status}");

            var items = acquisition.Items.ToList();
            foreach (var item in items)
            {
                await _acquisitionItemsRepository.DeleteAsync(item);
            }

            await _acquisitionRepository.DeleteAsync(acquisition);
            return new Response<int>(acquisition.Id, "Acquisition deleted successfully");
        }
    }

}