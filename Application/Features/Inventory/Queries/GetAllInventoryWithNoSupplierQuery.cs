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
    public class GetAllInventoryWithNoSupplierQuery : IRequest<Response<List<InventoryResponseDto>>>
    {
    }

    public class GetAllInventoryWithNoSupplierQueryHandler : IRequestHandler<GetAllInventoryWithNoSupplierQuery, Response<List<InventoryResponseDto>>>
    {
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMapper _mapper;
        public GetAllInventoryWithNoSupplierQueryHandler(IInventoryRepositoryAsync inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }
        public async Task<Response<List<InventoryResponseDto>>> Handle(GetAllInventoryWithNoSupplierQuery request, CancellationToken cancellationToken)
        {
            var list = await _inventoryRepository.GetByNoSupplierAsync();
            var mappedList = _mapper.Map<List<InventoryResponseDto>>(list);
            return new Response<List<InventoryResponseDto>>(mappedList);
        }
    }
}