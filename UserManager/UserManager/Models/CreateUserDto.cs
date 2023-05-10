namespace UserManager.Models
{
    public class CreateUserDto
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string? UserGroup { get; set; } = PredefinedUserGroup.User.ToString();
    }
}
