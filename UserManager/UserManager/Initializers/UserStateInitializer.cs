using System.ComponentModel;
using System.Reflection;
using UserManager.Models;

namespace UserManager.Initializers;

public static class UserStateInitializer
{
    public static IEnumerable<UserState> GetData()
    {
        yield return CreateUserState(PredefinedUserState.Active);
        yield return CreateUserState(PredefinedUserState.Blocked);
        yield return CreateUserState(PredefinedUserState.Initializing);
    }

    private static UserState CreateUserState(PredefinedUserState userState)
    {
        var descriptionAttribute = typeof(PredefinedUserState)
            .GetMember(userState.ToString())
            .First()
            .GetCustomAttribute<DescriptionAttribute>();

        var description = descriptionAttribute == null ? userState.ToString() : descriptionAttribute.Description;

        return new UserState
        {
            Id = (int)userState,
            Code = userState.ToString(),
            Description = description
        };
    }
}