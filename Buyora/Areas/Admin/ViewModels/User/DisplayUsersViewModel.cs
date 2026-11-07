namespace Buyora.Areas.Admin.ViewModels.User
{
    public class DisplayUsersViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsLocked { get; set; } = false;
    }
}
