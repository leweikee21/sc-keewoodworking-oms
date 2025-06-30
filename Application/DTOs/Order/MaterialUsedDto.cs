using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Inventory;

namespace Application.DTOs.Orders
{
    public class MaterialUsedDto
    {
        public InventoryResponseDto Inventory { get; set; }
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
