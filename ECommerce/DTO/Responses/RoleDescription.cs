namespace ECommerce.DTO.Responses
{
    public class RoleDescription
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public IList<EditRoleUserModel> Users { get; set; }
    }
}
