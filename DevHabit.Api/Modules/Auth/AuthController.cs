using DevHabit.Api.Database;
using DevHabit.Api.Modules.Auth.DTOs;
using DevHabit.Api.Modules.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevHabit.Api.Modules.Auth;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    ApplicationIdentityDbContext identityDb,
    ApplicationDbContext applicationDb) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        using IDbContextTransaction transaction = await identityDb.Database.BeginTransactionAsync();
        applicationDb.Database.SetDbConnection(identityDb.Database.GetDbConnection());
        await applicationDb.Database.UseTransactionAsync(transaction.GetDbTransaction());

        var identityUser = new IdentityUser
        {
            Email = request.Email,
            UserName = request.Name
        };

        IdentityResult result = await userManager.CreateAsync(identityUser, request.Password);

        if (!result.Succeeded)
        {
            var extensions = new Dictionary<string, object?> {
                {
                    "errors",
                    result.Errors.ToDictionary(e => e.Code, e => e.Description)
                }
            };

            return Problem(
                detail: "Unable to register user, please try again",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }

        User user = request.ToEntity();

        user.IdentityId = identityUser.Id;

        applicationDb.Users.Add(user);

        await applicationDb.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok(user.Id);
    }
}
