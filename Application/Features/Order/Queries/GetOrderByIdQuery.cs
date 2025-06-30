using Application.DTOs.Orders;
using Application.DTOs.User;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Queries
{
    public class GetOrderByIdQuery : IRequest<Response<OrderResponseDto>>
    {
        public int Id { get; set; }
    }

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Response<OrderResponseDto>>
    {
        private readonly IUserService _userService;
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IMaterialUsedRepositoryAsync _materialUsedRepository;
        private readonly IMapper _mapper;
        public GetOrderByIdQueryHandler(IUserService userService, IOrderRepositoryAsync orderRepository, IMaterialUsedRepositoryAsync materialUsedRepository, IMapper mapper)
        {
            _userService = userService;
            _orderRepository = orderRepository;
            _materialUsedRepository = materialUsedRepository;
            _mapper = mapper;
        }
        public async Task<Response<OrderResponseDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null) 
                throw new ApiException($"Order Not Found.");

            var CreatedBy = await _userService.GetUserByIdAsync(order.CreatedBy);
            if (order.LastModifiedBy != null)
            {
                var x = await _userService.GetUserByIdAsync(order.LastModifiedBy);
                order.LastModifiedBy = $@"{x.Data.FirstName} {x.Data.LastName}";
            }

            var mappedOrder = _mapper.Map<OrderResponseDto>(order);
            mappedOrder.CreatedBy = $"{CreatedBy.Data.FirstName} {CreatedBy.Data.LastName}";

            return new Response<OrderResponseDto>(mappedOrder);
        }
    }
}