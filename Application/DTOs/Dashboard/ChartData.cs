using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Dashboard
{
    public class ChartData
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Values { get; set; } = new();
        public string ChartType { get; set; } = "bar";
    }
}
