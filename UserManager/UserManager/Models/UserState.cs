using System.ComponentModel;

namespace UserManager.Models
{
    public enum PredefinedUserState
    {
        [Description("Active user")]
        Active = 1,

        [Description("Blocked user")]
        Blocked = 2,

        [Description("User is initializing")]
        Initializing = 3
    }

    public class UserState
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
