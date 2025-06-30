using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Inventory;

namespace Application.DTOs.Inbound
{
    public class InboundResponseDto
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public InventoryResponseDto Inventory { get; set; }
        public int Quantity { get; set; }
        public int RemainingQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public int AcquisitionId { get; set; }

    }
}
