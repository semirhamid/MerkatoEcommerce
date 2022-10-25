using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models
{
    public class UserLoginInfoModel : UserLoginInfo
    {
        public string loginProvider;
        public string providerKey;
        public string displayName;
        public UserLoginInfoModel(string loginProvider, string providerKey, string displayName) : 
            base(loginProvider, providerKey, displayName)
        {
            this.loginProvider = loginProvider;
            this.providerKey = providerKey;
            this.displayName = displayName;
        }
    }
}
