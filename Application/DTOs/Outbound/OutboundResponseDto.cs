using Application.DTOs.Inbound;
using Application.DTOs.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Outbound
{
    public class OutboundResponseDto
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public InventoryResponseDto Inventory { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
        public double TotalPrice { get; set; }
        public int? OrderId { get; set; }
        public InboundResponseDto Inbound { get; set; }
    }
}