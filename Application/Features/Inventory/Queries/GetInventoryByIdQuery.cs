using Application.DTOs.Inventory;
using Application.DTOs.Supplier;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inventory.Queries
{
    public class GetInventoryByIdQuery : IRequest<Response<InventoryResponseDto>>
    {
        public int Id { get; set; }
    }

    public class GetInventoryByIdQueryHandler : IRequestHandler<GetInventoryByIdQuery, Response<InventoryResponseDto>>
    {
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMapper _mapper;
        public GetInventoryByIdQueryHandler(IInventoryRepositoryAsync inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }
        public async Task<Response<InventoryResponseDto>> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(request.Id);
            if (inventory == null) throw new ApiException($"Inventory not found.");

            var mappedInventory = _mapper.Map<InventoryResponseDto>(inventory);
            return new Response<InventoryResponseDto>(mappedInventory);
        }
    }

}