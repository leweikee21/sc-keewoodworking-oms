using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Order
{
    public class OrderMonthlyTrendDto
    {
        public string Month { get; set; }
        public int TotalOrders { get; set; }
    }
}
