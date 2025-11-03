using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.UserAdministration.DTOs;
using XooCreator.BA.Features.UserAdministration.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.UserAdministration.Endpoints;

[Endpoint]
public class GetAllUsersEndpoint
{
    private readonly IUserAdministrationService _userAdministrationService;

    public GetAllUsersEndpoint(IUserAdministrationService userAdministrationService)
    {
        _userAdministrationService = userAdministrationService;
    }

    [Route("/api/user-administration/users")]
    [Authorize]
    public static async Task<Results<Ok<GetAllUsersResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromServices] GetAllUsersEndpoint ep,
        CancellationToken ct)
    {
        var result = await ep._userAdministrationService.GetAllUsersAsync(ct);
        return TypedResults.Ok(result);
    }
}

[Endpoint]
public class UpdateUserRoleEndpoint
{
    private readonly IUserAdministrationService _userAdministrationService;

    public UpdateUserRoleEndpoint(IUserAdministrationService userAdministrationService)
    {
        _userAdministrationService = userAdministrationService;
    }

    [Route("/api/user-administration/users/{userId}/role")]
    [Authorize]
    public static async Task<Results<Ok<UpdateUserRoleResponse>, UnauthorizedHttpResult, BadRequest<UpdateUserRoleResponse>, NotFound>> HandlePut(
        [FromRoute] Guid userId,
        [FromServices] UpdateUserRoleEndpoint ep,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken ct)
    {
        var result = await ep._userAdministrationService.UpdateUserRoleAsync(userId, request.Role, ct);
        
        if (!result.Success)
        {
            if (result.ErrorMessage?.Contains("not found") == true)
                return TypedResults.NotFound();
            
            return TypedResults.BadRequest(result);
        }

        return TypedResults.Ok(result);
    }
}

[Endpoint]
public class GetCurrentUserEndpoint
{
    private readonly IUserAdministrationService _userAdministrationService;

    public GetCurrentUserEndpoint(IUserAdministrationService userAdministrationService)
    {
        _userAdministrationService = userAdministrationService;
    }

    [Route("/api/user-administration/current-user")]
    [Authorize]
    public static async Task<Results<Ok<CurrentUserResponse>, UnauthorizedHttpResult, NotFound>> HandleGet(
        [FromServices] GetCurrentUserEndpoint ep,
        CancellationToken ct)
    {
        var currentUser = await ep._userAdministrationService.GetCurrentUserAsync(ct);
        
        if (currentUser == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(currentUser);
    }
}