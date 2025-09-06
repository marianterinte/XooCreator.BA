using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight;

[ApiController]
[Route("api/tree-of-light")]
// [Authorize] // Commented out for testing - will be enabled later
public class TreeOfLightController : ControllerBase
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;

    public TreeOfLightController(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [HttpGet("progress")]
    public async Task<ActionResult<List<TreeProgressDto>>> GetTreeProgress()
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var progress = await _service.GetTreeProgressAsync(userId.Value);
        return Ok(progress);
    }

    // Deprecated: returns completed stories (user story progress). Use GET /api/tree-of-light/user-progress instead.
    [HttpGet("stories")]
    [Obsolete("Use /api/tree-of-light/user-progress")] 
    public async Task<ActionResult<List<StoryProgressDto>>> GetStoryProgressLegacy()
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();
        var stories = await _service.GetStoryProgressAsync(userId.Value);
        return Ok(stories);
    }

    // New clearer route for user story completion progress
    [HttpGet("user-progress")]
    public async Task<ActionResult<List<StoryProgressDto>>> GetUserStoryProgress()
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();
        var stories = await _service.GetStoryProgressAsync(userId.Value);
        return Ok(stories);
    }

    [HttpGet("tokens")]
    public async Task<ActionResult<UserTokensDto>> GetUserTokens()
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var tokens = await _service.GetUserTokensAsync(userId.Value);
        return Ok(tokens);
    }

    [HttpGet("heroes")]
    public async Task<ActionResult<List<HeroDto>>> GetHeroProgress()
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var heroes = await _service.GetHeroProgressAsync(userId.Value);
        return Ok(heroes);
    }

    [HttpGet("hero-tree")]
    public async Task<ActionResult<List<HeroTreeNodeDto>>> GetHeroTreeProgress()
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var heroTree = await _service.GetHeroTreeProgressAsync(userId.Value);
        return Ok(heroTree);
    }

    [HttpPost("complete-story")]
    public async Task<ActionResult<CompleteStoryResponse>> CompleteStory([FromBody] CompleteStoryRequest request)
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var result = await _service.CompleteStoryAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("unlock-hero-tree-node")]
    public async Task<ActionResult<UnlockHeroTreeNodeResponse>> UnlockHeroTreeNode([FromBody] UnlockHeroTreeNodeRequest request)
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var result = await _service.UnlockHeroTreeNodeAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("transform-hero")]
    public async Task<ActionResult<TransformToHeroResponse>> TransformToHero([FromBody] TransformToHeroRequest request)
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var result = await _service.TransformToHeroAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("reset-progress")]
    public async Task<ActionResult<ResetProgressResponse>> ResetProgress()
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var result = await _service.ResetUserProgressAsync(userId.Value);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
