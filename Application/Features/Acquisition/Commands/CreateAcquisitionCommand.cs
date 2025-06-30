using Application.DTOs.Account;
using Application.DTOs.Acquisition;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Acquisition.Commands
{
    public partial class CreateAcquisitionCommand : IRequest<Response<int>>
    {
        public int SupplierId { get; set; }
        public bool isDraft { get; set; }
        public List<AcquisitionItemDto> Items { get; set; }
    }

    public class CreateAcquisitionCommandHandler : IRequestHandler<CreateAcquisitionCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IAcquisitionItemRepositoryAsync _acquisitionItemRepository;
        private readonly IMapper _mapper;

        public CreateAcquisitionCommandHandler(IAuthenticatedUserService authenticatedUser, IAcquisitionRepositoryAsync acquisitionRepository, IAcquisitionItemRepositoryAsync acquisitionItemRepository, IMapper mapper)
        {
            _authenticatedUser = authenticatedUser;
            _acquisitionRepository = acquisitionRepository;
            _acquisitionItemRepository = acquisitionItemRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateAcquisitionCommand command, CancellationToken cancellationToken)
        {
            var acquisition = new Domain.Entities.Acquisition
            {
                SupplierId = command.SupplierId,
                Status = command.isDraft ? "Draft" : "Created",
                TotalItems = command.Items.Count,
                Created = DateTime.UtcNow,
                CreatedBy = _authenticatedUser.UserId
            };

            await _acquisitionRepository.AddAsync(acquisition);

            foreach (var item in command.Items)
            {
                var acquisitionItem = new AcquisitionItem
                {
                    AcquisitionId = acquisition.Id,
                    InventoryId = item.InventoryId,
                    Quantity = item.Quantity
                };
                await _acquisitionItemRepository.AddAsync(acquisitionItem);
            }

            return new Response<int>(acquisition.Id, "Acquisitions created successfully");
        }
    }
}