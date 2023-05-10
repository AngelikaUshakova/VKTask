using System.ComponentModel;
using System.Reflection;
using UserManager.Models;

namespace UserManager.Initializers;

public static class UserGroupInitializer
{
    public static IEnumerable<UserGroup> GetData()
    {
        yield return CreateUserGroup(PredefinedUserGroup.Admin);
        yield return CreateUserGroup(PredefinedUserGroup.User);
    }

    private static UserGroup CreateUserGroup(PredefinedUserGroup userGroup)
    {
        var descriptionAttribute = typeof(PredefinedUserGroup).GetMember(userGroup.ToString())
            .First()
            .GetCustomAttribute<DescriptionAttribute>();

        var description = descriptionAttribute == null ? userGroup.ToString() : descriptionAttribute.Description;
      
        return new UserGroup
        {
            Id = (int)userGroup,
            Code = userGroup.ToString(),
            Description = description
        };
    }
}