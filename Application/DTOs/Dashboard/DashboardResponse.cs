using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Dashboard
{
    public class DashboardResponse
    {
        public List<DashboardCard> SummaryCards { get; set; } = new();
        public ChartData Chart { get; set; } = new();
        public List<ChartData> AdditionalCharts { get; set; } = new();
        public List<string> Notifications { get; set; } = new();
    }
}
