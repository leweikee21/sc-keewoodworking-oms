using Application.DTOs.Outbound;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Outbound.Queries
{
    public class GetAllOutboundQuery : IRequest<object>
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? Category { get; set; }
    }

    public class GetAllOutboundQueryHandler : IRequestHandler<GetAllOutboundQuery, object>
    {
        private readonly IOutboundRepositoryAsync _outboundRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public GetAllOutboundQueryHandler(IOutboundRepositoryAsync outboundRepository, IUserService userService, IMapper mapper)
        {
            _outboundRepository = outboundRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetAllOutboundQuery request, CancellationToken cancellationToken)
        {
            var validFilter = _mapper.Map<GetAllOutboundParameter>(request);
            var outbounds = await _outboundRepository.GetAllOutboundAsync(validFilter);
            var mappedOutbound = _mapper.Map<List<OutboundResponseDto>>(outbounds.Data);

            foreach (var outbound in mappedOutbound)
            {
                var CreatedBy = await _userService.GetUserByIdAsync(outbound.CreatedBy);
                if (outbound.LastModifiedBy != null)
                {
                    var x = await _userService.GetUserByIdAsync(outbound.LastModifiedBy);
                    outbound.LastModifiedBy = $@"{x.Data.FirstName} {x.Data.LastName}";
                }
                outbound.CreatedBy = $@"{CreatedBy.Data.FirstName} {CreatedBy.Data.LastName}";
            }

            var outboundResponse = new PagedResponse<List<OutboundResponseDto>>(
                mappedOutbound,
                outbounds.PageNumber,
                outbounds.PageSize,
                outbounds.TotalCount
            );

            return outboundResponse;
        }
    }
}