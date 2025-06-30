using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Acquisition
{
    public class AcquisitionResponseDto
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string SupplierName { get; set; }
        public string? filePath { get; set; }
        public List<AcquisitionItemDto> Items { get; set; } = new List<AcquisitionItemDto>();
    }
}
