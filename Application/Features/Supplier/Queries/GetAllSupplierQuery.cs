using Application.DTOs.Supplier;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Supplier.Queries
{
    public class GetAllSupplierQuery : IRequest<object>
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetAllSupplierQueryHandler : IRequestHandler<GetAllSupplierQuery, object>
    {
        private readonly ISupplierRepositoryAsync _supplierRepository;
        private readonly IMapper _mapper;
        public GetAllSupplierQueryHandler(ISupplierRepositoryAsync supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetAllSupplierQuery request, CancellationToken cancellationToken)
        {
            var validFilter = _mapper.Map<GetAllSupplierParameter>(request);
            var supplier = await _supplierRepository.GetAllSupplierAsync(validFilter);
            var mappedSupplier = _mapper.Map<List<SupplierResponseDto>>(supplier.Data);

            var supplierResponse = new PagedResponse<List<SupplierResponseDto>>(
                mappedSupplier,
                supplier.PageNumber,
                supplier.PageSize,
                supplier.TotalCount
            );

            return supplierResponse;
        }
    }

}