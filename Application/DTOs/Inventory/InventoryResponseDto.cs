using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventory
{
    public class InventoryResponseDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }    
        public string Category { get; set; }
        public string Unit { get; set; }
        public int TotalQty { get; set; }
        public int ReservedQty { get; set; }
        public int AvailableQty { get; set; }
        public int MinQty { get; set; }
        public decimal LastUnitPrice { get; set; }
        public decimal AverageUnitPrice { get; set; }
        public DateTime LastInDate { get; set; }
        public DateTime LastOutDate { get; set; }
        public string SupplierName { get; set; }
    }
}
