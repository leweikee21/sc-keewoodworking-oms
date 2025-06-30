using Application.DTOs.Supplier;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Supplier.Queries
{
    public class GetSupplierByIdQuery : IRequest<Response<SupplierResponseDto>>
    {
        public int Id { get; set; }
    }

    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Response<SupplierResponseDto>>
    {
        private readonly ISupplierRepositoryAsync _supplierRepository;
        private readonly IMapper _mapper;
        public GetSupplierByIdQueryHandler(ISupplierRepositoryAsync supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }
        public async Task<Response<SupplierResponseDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _supplierRepository.GetByIdAsync(request.Id);
            if (supplier == null) 
                throw new ApiException($"Supplier Not Found.");

            var mappedSupplier = _mapper.Map<SupplierResponseDto>(supplier);
            return new Response<SupplierResponseDto>(mappedSupplier);
        }
    }

}