using Application.DTOs.Orders;
using Application.DTOs.User;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Commands
{
    public class UpdateOrderStatusCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime? ActualDelDate { get; set; }
    }

    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser; 
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IMapper _mapper;

        public UpdateOrderStatusCommandHandler(IAuthenticatedUserService authenticateUser, IOrderRepositoryAsync orderRepository, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateOrderStatusCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(command.Id);

            if (order == null)
            {
                throw new ApiException("Order not found");
            }

            order.Status = command.Status;
            if (command.ActualDelDate != null) 
                order.ActualDelDate = DateTime.SpecifyKind(command.ActualDelDate.Value, DateTimeKind.Utc);
            order.LastModified = DateTime.UtcNow;
            order.LastModifiedBy = _authenticatedUser.UserId;

            await _orderRepository.UpdateAsync(order);
            return new Response<int>(order.Id, "Order status updated.");
        }
    }
}