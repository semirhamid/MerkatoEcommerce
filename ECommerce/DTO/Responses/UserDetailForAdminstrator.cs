using ECommerce.Models;
using ECommerceModels;

namespace ECommerce.DTO.Responses
{
    public class UserDetailForAdminstrator : UserDashboardModel
    {
        public UserProfile User { get; set; }
        
    }
}
