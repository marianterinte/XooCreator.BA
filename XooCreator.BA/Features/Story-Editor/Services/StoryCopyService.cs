using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services.Cloning;
using XooCreator.BA.Features.UserAdministration.Repositories;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryCopyService
{
    Task<StoryCraft> CreateCopyFromCraftAsync(
        StoryCraft sourceCraft,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false);

    Task<StoryCraft> CreateCopyFromDefinitionAsync(
        StoryDefinition definition,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false,
        bool lightChanges = false);
}

/// <summary>
/// Refactored service responsible for cloning story structures for Copy/Fork/New Version flows.
/// Now uses unified cloning services to eliminate code duplication.
/// </summary>
public class StoryCopyService : IStoryCopyService
{
    private readonly IStoryCraftsRepository _craftsRepository;
    private readonly ILogger<StoryCopyService> _logger;
    private readonly IStorySourceMapper _sourceMapper;
    private readonly IStoryCloner _cloner;
    private readonly IUserAdministrationRepository _userRepository;

    public StoryCopyService(
        IStoryCraftsRepository craftsRepository,
        ILogger<StoryCopyService> logger,
        IStorySourceMapper sourceMapper,
        IStoryCloner cloner,
        IUserAdministrationRepository userRepository)
    {
        _craftsRepository = craftsRepository;
        _logger = logger;
        _sourceMapper = sourceMapper;
        _cloner = cloner;
        _userRepository = userRepository;
    }

    public async Task<StoryCraft> CreateCopyFromCraftAsync(
        StoryCraft sourceCraft,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false)
    {
        ArgumentNullException.ThrowIfNull(sourceCraft);
        ValidateInputs(ownerUserId, newStoryId);

        // Detect if this is a fork (owner is different from source owner)
        bool isFork = sourceCraft.OwnerUserId != ownerUserId;

        // Map source to unified clone data
        var cloneData = _sourceMapper.MapFromCraft(sourceCraft, isCopy, isFork);
        
        // If this is a fork, set AuthorName to the owner's name
        if (isFork && string.IsNullOrWhiteSpace(cloneData.AuthorName))
        {
            var owner = await _userRepository.GetUserByIdAsync(ownerUserId, ct);
            
            if (owner != null && !string.IsNullOrWhiteSpace(owner.Name))
            {
                cloneData.AuthorName = owner.Name;
            }
        }
        
        // Create new craft from clone data
        var craft = _cloner.CreateCraft(cloneData, ownerUserId, newStoryId);
        
        await _craftsRepository.SaveAsync(craft, ct);

        _logger.LogInformation(
            "Story copied from draft: sourceStoryId={SourceStoryId} -> newStoryId={NewStoryId} owner={OwnerId} isCopy={IsCopy}",
            sourceCraft.StoryId,
            newStoryId,
            ownerUserId,
            isCopy);

        return craft;
    }

    public async Task<StoryCraft> CreateCopyFromDefinitionAsync(
        StoryDefinition definition,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false,
        bool lightChanges = false)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ValidateInputs(ownerUserId, newStoryId);

        // Detect if this is a fork (owner is different from CreatedBy)
        bool isFork = definition.CreatedBy.HasValue && definition.CreatedBy.Value != ownerUserId;

        // Map source to unified clone data
        var cloneData = _sourceMapper.MapFromDefinition(definition, isCopy, isFork);
        
        // If this is a fork, set AuthorName to the owner's name
        if (isFork && string.IsNullOrWhiteSpace(cloneData.AuthorName))
        {
            var owner = await _userRepository.GetUserByIdAsync(ownerUserId, ct);
            
            if (owner != null && !string.IsNullOrWhiteSpace(owner.Name))
            {
                cloneData.AuthorName = owner.Name;
            }
        }
        
        // Create new craft from clone data
        var craft = _cloner.CreateCraft(cloneData, ownerUserId, newStoryId);
        craft.LightChanges = lightChanges;
        
        await _craftsRepository.SaveAsync(craft, ct);

        _logger.LogInformation(
            "Story copied from published definition: sourceStoryId={SourceStoryId} -> newStoryId={NewStoryId} owner={OwnerId} isCopy={IsCopy}",
            definition.StoryId,
            newStoryId,
            ownerUserId,
            isCopy);

        return craft;
    }

    private static void ValidateInputs(Guid ownerUserId, string newStoryId)
    {
        if (ownerUserId == Guid.Empty)
        {
            throw new ArgumentException("ownerUserId must be valid", nameof(ownerUserId));
        }

        if (string.IsNullOrWhiteSpace(newStoryId))
        {
            throw new ArgumentException("newStoryId must be provided", nameof(newStoryId));
        }
    }
}
