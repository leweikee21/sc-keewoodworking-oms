using Application.DTOs.Inbound;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inbound.Queries
{
    public class GetAllInboundQuery : IRequest<object>
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? Category { get; set; }
    }

    public class GetAllInboundQueryHandler : IRequestHandler<GetAllInboundQuery, object>
    {
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public GetAllInboundQueryHandler(IInboundRepositoryAsync inboundRepository, IUserService userService, IMapper mapper)
        {
            _inboundRepository = inboundRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetAllInboundQuery request, CancellationToken cancellationToken)
        {
            var validFilter = _mapper.Map<GetAllInboundParameter>(request);
            var inbounds = await _inboundRepository.GetAllInboundAsync(validFilter);
            var mappedInbound = _mapper.Map<List<InboundResponseDto>>(inbounds.Data);

            foreach (var inbound in mappedInbound)
            {
                var CreatedBy = await _userService.GetUserByIdAsync(inbound.CreatedBy);
                if (inbound.LastModifiedBy != null)
                {
                    var x = await _userService.GetUserByIdAsync(inbound.LastModifiedBy);
                    inbound.LastModifiedBy = $@"{x.Data.FirstName} {x.Data.LastName}";
                }

                inbound.CreatedBy = $@"{CreatedBy.Data.FirstName} {CreatedBy.Data.LastName}";
            }

            var inboundResponse = new PagedResponse<List<InboundResponseDto>>(
                mappedInbound,
                inbounds.PageNumber,
                inbounds.PageSize,
                inbounds.TotalCount
            );

            return inboundResponse;
        }
    }

}