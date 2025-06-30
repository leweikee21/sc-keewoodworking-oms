using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Filters;

namespace Application.Features.Acquisition.Queries
{
    public class GetAllAcquisitionParameter : RequestParameter
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
    }
}
