using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventory
{
    public class ReactivateInventoryRequest
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public int TotalQty { get; set; }
        public int MinQty { get; set; }
        public decimal UnitPrice { get; set; }
        public int? SupplierId { get; set; }

    }
}
