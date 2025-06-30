using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Inbound : AuditableBaseEntity
    {
        public int InventoryId { get; set; }        
        public Inventory Inventory { get; set; }    
        
        public int Quantity { get; set; }
        public int RemainingQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Remark { get; set; }

        public int? AcquisitionId { get; set; }
        public Acquisition Acquisition { get; set; }

    }
}
