using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Filters;

namespace Application.Features.Supplier.Queries
{
    public class GetAllSupplierParameter : RequestParameter
    {
        public string? Search { get; set; }
    }
}
