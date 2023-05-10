using System.ComponentModel;

namespace UserManager.Models
{
    public enum PredefinedUserGroup
    {
        [Description("System administrator")]
        Admin = 1,

        [Description("Regular user")]
        User = 2
    }

    public class UserGroup
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
