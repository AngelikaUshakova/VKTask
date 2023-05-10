namespace UserManager.Models;

public class UserDto
{
    public int Id { get; set; }

    public string Login { get; set; }

    public DateTime CreatedDate { get; set; }

    public UserGroupDto UserGroup { get; set; }

    public UserStateDto UserState { get; set; }
}