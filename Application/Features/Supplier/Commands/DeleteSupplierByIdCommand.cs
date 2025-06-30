using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Supplier.Commands
{
    public class DeleteSupplierByIdCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteSupplierByIdCommandHandler : IRequestHandler<DeleteSupplierByIdCommand, Response<int>>
    {
        private readonly ISupplierRepositoryAsync _supplierRepository;

        public DeleteSupplierByIdCommandHandler(ISupplierRepositoryAsync supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Response<int>> Handle(DeleteSupplierByIdCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _supplierRepository.GetByIdAsync(request.Id);

            if (supplier == null) 
                throw new ApiException($"Supplier not found.");

            if (supplier.Inventories.Any())
                throw new ApiException("Cannot be deleted as it has linked inventories.");

            await _supplierRepository.DeleteAsync(supplier);
            return new Response<int>(supplier.Id, "Supplier deleted successfully");
        }
    }

}