using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Supplier : AuditableBaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPerson { get; set; }
        public List<Inventory> Inventories { get; set; } = new List<Inventory>();
        public List<Acquisition> Acquisitions { get; set; } = new List<Acquisition>();
    }
}
