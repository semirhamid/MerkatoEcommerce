using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceModels
{
    public class UserDashboardModel
    {
        public double TotalProduct { get; set; }
        public double TotalOrder { get; set; }
        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }
    }
}
