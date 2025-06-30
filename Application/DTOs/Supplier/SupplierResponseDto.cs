using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Supplier
{
    public class SupplierResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPerson { get; set; }
    }
}