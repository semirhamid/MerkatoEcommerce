using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models
{
    public class ExternalLoginModel 
    { 
        public string LoginProvider { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }
}
