using Application.DTOs.Inventory;
using Application.Enums;
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
    public class GetAllInventoryQuery : IRequest<object>
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? StockLevel { get; set; }
        public string? Category { get; set; }
    }

    public class GetAllInventoryQueryHandler : IRequestHandler<GetAllInventoryQuery, object>
    {
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMapper _mapper;
        public GetAllInventoryQueryHandler(IInventoryRepositoryAsync inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
        {
            var validFilter = _mapper.Map<GetAllInventoryParameter>(request);
            var inventory = await _inventoryRepository.GetAllInventoryAsync(validFilter);
            var mappedInventory = _mapper.Map<List<InventoryResponseDto>>(inventory.Data);

            var inventoryResponse = new PagedResponse<List<InventoryResponseDto>>(
                mappedInventory,
                inventory.PageNumber,
                inventory.PageSize,
                inventory.TotalCount
            );

            return inventoryResponse;
        }
    }

}