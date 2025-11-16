using System.Linq.Expressions;

namespace DevHabit.Api.Modules.Users.DTOs;

internal static class UserQueries
{
    public static Expression<Func<User, UserResponse>> ProjectToResponse()
    {
        return u => new UserResponse
        {
            Id = u.Id,
            Email = u.Email,
            Name = u.Name,
            CreatedAtUtc = u.CreatedAtUtc,
            UpdatedAtUtc = u.UpdatedAtUtc,
        };
    }
}
