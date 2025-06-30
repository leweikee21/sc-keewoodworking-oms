using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class MaterialUsed : AuditableBaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
