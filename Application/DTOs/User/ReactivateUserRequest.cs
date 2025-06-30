using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class ReactivateUserRequest
    {
        public string PhoneNumber { get; set; }
        public string TempPassword { get; set; }
        public List<string> Roles { get; set; }
    }
}
