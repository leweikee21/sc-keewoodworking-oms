using Application.DTOs.Acquisition;
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

namespace Application.Features.Acquisition.Queries
{
    public class GetAllAcquisitionQuery : IRequest<object>
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? Status { get; set; }
    }

    public class GetAllAcquisitionQueryHandler : IRequestHandler<GetAllAcquisitionQuery, object>
    {
        private readonly IUserService _userService;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IMapper _mapper;
        public GetAllAcquisitionQueryHandler(IUserService userService, IAcquisitionRepositoryAsync acquisitionRepository, IMapper mapper)
        {
            _userService = userService;
            _acquisitionRepository = acquisitionRepository;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetAllAcquisitionQuery request, CancellationToken cancellationToken)
        {
            var validFilter = _mapper.Map<GetAllAcquisitionParameter>(request);
            var acquisition = await _acquisitionRepository.GetAllAcquisitionAsync(validFilter);
            var mappedAcquisition = _mapper.Map<List<AcquisitionResponseDto>>(acquisition.Data);

            foreach (var ac in mappedAcquisition)
            {
                var CreatedBy = await _userService.GetUserByIdAsync(ac.CreatedBy);
                if (ac.LastModifiedBy != null)
                {
                    var x = await _userService.GetUserByIdAsync(ac.LastModifiedBy);
                    ac.LastModifiedBy = $@"{x.Data.FirstName} {x.Data.LastName}";
                }

                ac.CreatedBy = $@"{CreatedBy.Data.FirstName} {CreatedBy.Data.LastName}";
            }

            var acquisitionResponse = new PagedResponse<List<AcquisitionResponseDto>>(
                mappedAcquisition,
                acquisition.PageNumber,
                acquisition.PageSize,
                acquisition.TotalCount
            );

            return acquisitionResponse;
        }
    }

}