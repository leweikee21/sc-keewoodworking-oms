using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Outbound : AuditableBaseEntity
    {
        public int InventoryId { get; set; }        
        public Inventory Inventory { get; set; }

        public int Quantity { get; set; }
        public string? Remark { get; set; }
        public decimal TotalPrice { get; set; }

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public int InboundId { get; set; }
        public Inbound Inbound { get; set; }
    }
}
