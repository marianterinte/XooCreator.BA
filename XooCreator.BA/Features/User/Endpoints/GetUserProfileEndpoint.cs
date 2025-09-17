//Obsolete: The Endpoint feature is deprecated.
    
    

//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;
//using XooCreator.BA.Infrastructure.Endpoints;
//using XooCreator.BA.Infrastructure;

//namespace XooCreator.BA.Features.User.Endpoints;

//[Endpoint]
//public class GetUserProfileEndpoint
//{
//    private readonly IUserProfileService _userProfileService;
//    private readonly IUserContextService _userContext;

//    public GetUserProfileEndpoint(IUserProfileService userProfileService, IUserContextService userContext)
//    {
//        _userProfileService = userProfileService;
//        _userContext = userContext;
//    }

//    [Route("/api/user/profile")] // GET
//    public static async Task<Results<Ok<GetUserProfileResponse>, UnauthorizedHttpResult, NotFound>> HandleGet(
//        [FromServices] GetUserProfileEndpoint ep)
//    {
//        var userId = await ep._userContext.GetUserIdAsync();
//        if (userId == null)
//            return TypedResults.Unauthorized();

//        var result = await ep._userProfileService.GetUserProfileAsync(userId.Value);

//        if (!result.Success || result.User == null)
//            return TypedResults.NotFound();

//        return TypedResults.Ok(result);
//    }
//}
