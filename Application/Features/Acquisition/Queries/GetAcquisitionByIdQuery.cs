using Application.DTOs.Acquisition;
using Application.DTOs.Inventory;
using Application.DTOs.Supplier;
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

namespace Application.Features.Acquisition.Queries
{
    public class GetAcquisitionByIdQuery : IRequest<Response<AcquisitionResponseDto>>
    {
        public int Id { get; set; }
    }

    public class GetAcquisitionByIdQueryHandler : IRequestHandler<GetAcquisitionByIdQuery, Response<AcquisitionResponseDto>>
    {
        private readonly IUserService _userService;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IMapper _mapper;
        public GetAcquisitionByIdQueryHandler(IUserService userService, IAcquisitionRepositoryAsync acquisitionRepository, IMapper mapper)
        {
            _userService = userService;
            _acquisitionRepository = acquisitionRepository;
            _mapper = mapper;
        }
        public async Task<Response<AcquisitionResponseDto>> Handle(GetAcquisitionByIdQuery request, CancellationToken cancellationToken)
        {
            var acquisition = await _acquisitionRepository.GetByIdAsync(request.Id);
            if (acquisition == null) 
                throw new ApiException($"Acquisition Not Found.");

            var CreatedBy = await _userService.GetUserByIdAsync(acquisition.CreatedBy);
            if (acquisition.LastModifiedBy != null)
            {
                var x = await _userService.GetUserByIdAsync(acquisition.LastModifiedBy);
                acquisition.LastModifiedBy = $@"{x.Data.FirstName} {x.Data.LastName}";
            }

            var mappedAcquisition = _mapper.Map<AcquisitionResponseDto>(acquisition);
            mappedAcquisition.CreatedBy = $"{CreatedBy.Data.FirstName} {CreatedBy.Data.LastName}";
            
            return new Response<AcquisitionResponseDto>(mappedAcquisition);
        }
    }
}