using Application.DTOs.Orders;
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

namespace Application.Features.Order.Queries
{
    public class GetAllOrderQuery : IRequest<object>
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? Status { get; set; }
    }

    public class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQuery, object>
    {
        private readonly IUserService _userService;
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IMapper _mapper;
        public GetAllOrderQueryHandler(IUserService userService, IOrderRepositoryAsync orderRepository, IMapper mapper)
        {
            _userService = userService;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            var validFilter = _mapper.Map<GetAllOrderParameter>(request);
            var order = await _orderRepository.GetAllOrderAsync(validFilter);
            var mappedOrder = _mapper.Map<List<OrderResponseDto>>(order.Data);

            foreach (var o in mappedOrder)
            {
                var CreatedBy = await _userService.GetUserByIdAsync(o.CreatedBy);
                if (o.LastModifiedBy != null)
                {
                    var x = await _userService.GetUserByIdAsync(o.LastModifiedBy);
                    o.LastModifiedBy = $@"{x.Data.FirstName} {x.Data.LastName}";
                }

                o.CreatedBy = $@"{CreatedBy.Data.FirstName} {CreatedBy.Data.LastName}";
            }

            var orderResponse = new PagedResponse<List<OrderResponseDto>>(
                mappedOrder,
                order.PageNumber,
                order.PageSize,
                order.TotalCount
            );

            return orderResponse;
        }
    }

}