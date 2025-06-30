using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order : AuditableBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; } 
        public string Model { get; set; }
        public string ModelCode { get; set; }
        public string ModelCategory { get; set; }
        
        public string Status { get; set; }
        
        public DateTime ReceivedDate { get; set; }
        public DateTime RequiredDelDate { get; set; }
        public DateTime? ActualDelDate { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public decimal UnitWages { get; set; }
        public decimal OtherCost { get; set; }
        public decimal HardwareCost { get; set; }

        public decimal MaterialCost { get; set; }
        public decimal TotalRevenue { get; set; }
        public string? Remark { get; set; }
        public string? filePath { get; set; }

        public List<MaterialUsed> MaterialsUsed { get; set; } = new List<MaterialUsed>();
    }
}
