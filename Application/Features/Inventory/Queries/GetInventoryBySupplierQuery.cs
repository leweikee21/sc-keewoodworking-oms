using Application.DTOs.Inventory;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inventory.Queries
{
    public class GetInventoryBySupplierQuery : IRequest<Response<List<InventoryResponseDto>>>
    {
        public int SupplierId { get; set; }
    }

    public class GetInventoryBySupplierQueryHandler : IRequestHandler<GetInventoryBySupplierQuery, Response<List<InventoryResponseDto>>>
    {
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMapper _mapper;
        public GetInventoryBySupplierQueryHandler(IInventoryRepositoryAsync inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }
        public async Task<Response<List<InventoryResponseDto>>> Handle(GetInventoryBySupplierQuery request, CancellationToken cancellationToken)
        {
            var list = await _inventoryRepository.GetBySupplierAsync(request.SupplierId);
            var mappedList = _mapper.Map<List<InventoryResponseDto>>(list);
            return new Response<List<InventoryResponseDto>>(mappedList);
        }
    }
}