using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Filters;

namespace Application.Features.Inbound.Queries
{
    public class GetAllInboundParameter : RequestParameter
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
    }
}
