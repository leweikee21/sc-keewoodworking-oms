using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class AcquisitionItem : AuditableBaseEntity
    {
        public int AcquisitionId { get; set; }
        public Acquisition Acquisition { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public int Quantity { get; set; }
    }
}
