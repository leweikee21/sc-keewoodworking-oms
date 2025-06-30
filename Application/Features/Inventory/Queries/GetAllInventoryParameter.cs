using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Filters;

namespace Application.Features.Inventory.Queries
{
    public class GetAllInventoryParameter : RequestParameter
    {
        public string? Search { get; set; }
        public string? StockLevel { get; set; }
        public string? Category { get; set; }
    }
}
