using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Stories;

[ApiController]
[Route("api/stories")]
// [Authorize] // Commented out for testing - will be enabled later
public class StoriesController : ControllerBase
{
    private readonly IStoriesService _service;
    private readonly IUserContextService _userContext;

    public StoriesController(IStoriesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<ActionResult<GetStoriesResponse>> GetAllStories()
    {
        var result = await _service.GetAllStoriesAsync();
        return Ok(result);
    }

    [HttpGet("{storyId}")]
    public async Task<ActionResult<GetStoryByIdResponse>> GetStoryById(string storyId)
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var result = await _service.GetStoryByIdAsync(userId.Value, storyId);
        
        if (result.Story == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("mark-tile-read")]
    public async Task<ActionResult<MarkTileAsReadResponse>> MarkTileAsRead([FromBody] MarkTileAsReadRequest request)
    {
        var userId = await _userContext.GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var result = await _service.MarkTileAsReadAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
