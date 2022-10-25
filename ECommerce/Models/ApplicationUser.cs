using Microsoft.AspNetCore.Identity;
using System;

namespace ECommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday  { get; set; }
        public string? Gender { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

    }
}
