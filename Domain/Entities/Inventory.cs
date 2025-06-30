using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Inventory : AuditableBaseEntity
    {
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

        public DateTime? LastInDate { get; set; }
        public DateTime? LastOutDate { get; set; }

        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public List<Inbound> Inbounds { get; set; } = new List<Inbound>();
        public List<Outbound> Outbounds { get; set; } = new List<Outbound>();

        public bool IsDeleted { get; set; }
    }
}
