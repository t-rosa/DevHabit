using DevHabit.Api.Modules.Auth.DTOs;

namespace DevHabit.Api.Modules.Users;

public static class UserMappings
{
    extension(RegisterRequest request)
    {
        public User ToEntity()
        {
            return new User
            {
                Id = $"u_{Guid.CreateVersion7()}",
                Name = request.Name,
                Email = request.Email,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}
