using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceModels
{
    public class AdminstratorDashboardModel
    {
        public double TotalProduct { get; set; }
        public double TotalOrder { get; set; }
        public int TotalUsers { get; set; }
        public List<Product> LastTenProducts { get; set; }
        public List<Order> LastSeveDaysOrders { get; set; }
        public double Totalsales { get; set; }
        public double TodaysSales { get; set; }

    }
}
