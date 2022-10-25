using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceModels
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Date { get; set; }
    }
}
