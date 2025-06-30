using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Acquisition : AuditableBaseEntity
    {
        public string Status { get; set; }
        public int TotalItems { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public string? filePath { get; set; }

        public List<AcquisitionItem> Items { get; set; } = new List<AcquisitionItem>();
    }
}
