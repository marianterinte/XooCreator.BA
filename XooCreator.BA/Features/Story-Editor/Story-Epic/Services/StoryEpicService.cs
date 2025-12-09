using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicService : IStoryEpicService
{
    private readonly IStoryEpicRepository _repository;
    private readonly XooDbContext _context;

    public StoryEpicService(IStoryEpicRepository repository, XooDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task EnsureEpicAsync(Guid ownerUserId, string epicId, string name, CancellationToken ct = default)
    {
        var exists = await _repository.ExistsAsync(epicId, ct);
        if (!exists)
        {
            await _repository.CreateAsync(ownerUserId, epicId, name, ct);
        }
    }

    public async Task SaveEpicAsync(Guid ownerUserId, string epicId, StoryEpicDto dto, CancellationToken ct = default)
    {
        // Verify ownership
        var existing = await _repository.GetAsync(epicId, ct);
        if (existing != null && existing.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        // Ensure epic exists
        if (existing == null)
        {
            existing = await _repository.CreateAsync(ownerUserId, epicId, dto.Name, ct);
        }

        // Update basic properties
        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Status = dto.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        // Update regions
        await UpdateRegionsAsync(existing, dto.Regions, ct);

        // Update story nodes
        await UpdateStoryNodesAsync(existing, dto.Stories, ct);

        // Update unlock rules
        await UpdateUnlockRulesAsync(existing, dto.Rules, ct);

        // Save changes
        await _repository.SaveAsync(existing, ct);
    }

    public async Task<StoryEpicDto?> GetEpicAsync(string epicId, CancellationToken ct = default)
    {
        var epic = await _repository.GetFullAsync(epicId, ct);
        if (epic == null) return null;

        return MapToDto(epic);
    }

    public async Task<StoryEpicStateDto?> GetEpicStateAsync(string epicId, CancellationToken ct = default)
    {
        var epic = await GetEpicAsync(epicId, ct);
        if (epic == null) return null;

        var preview = GeneratePreview(epic);

        return new StoryEpicStateDto
        {
            Epic = epic,
            Preview = preview
        };
    }

    public async Task<List<StoryEpicListItemDto>> ListEpicsByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
    {
        var epics = await _repository.ListByOwnerAsync(ownerUserId, ct);
        
        return epics.Select(e => new StoryEpicListItemDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Status = e.Status,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            StoryCount = e.StoryNodes.Count,
            RegionCount = e.Regions.Count
        }).ToList();
    }

    public async Task DeleteEpicAsync(Guid ownerUserId, string epicId, CancellationToken ct = default)
    {
        // Verify ownership
        var epic = await _repository.GetAsync(epicId, ct);
        if (epic == null)
        {
            throw new InvalidOperationException($"Epic '{epicId}' not found");
        }

        if (epic.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        await _repository.DeleteAsync(epicId, ct);
    }

    private StoryEpicDto MapToDto(Data.StoryEpic epic)
    {
        return new StoryEpicDto
        {
            Id = epic.Id,
            Name = epic.Name,
            Description = epic.Description,
            Status = epic.Status,
            Regions = epic.Regions.Select(r => new StoryEpicRegionDto
            {
                Id = r.RegionId,
                Label = r.Label,
                ImageUrl = r.ImageUrl,
                SortOrder = r.SortOrder,
                IsLocked = r.IsLocked,
                X = r.X,
                Y = r.Y
            }).ToList(),
            Stories = epic.StoryNodes.Select(sn => new StoryEpicStoryNodeDto
            {
                StoryId = sn.StoryId,
                RegionId = sn.RegionId,
                RewardImageUrl = sn.RewardImageUrl,
                SortOrder = sn.SortOrder,
                X = sn.X,
                Y = sn.Y
            }).ToList(),
            Rules = epic.UnlockRules.Select(r => new StoryEpicUnlockRuleDto
            {
                Type = r.Type,
                FromId = r.FromId,
                ToRegionId = r.ToRegionId,
                RequiredStories = r.RequiredStoriesCsv?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                MinCount = r.MinCount,
                StoryId = r.StoryId,
                SortOrder = r.SortOrder
            }).ToList()
        };
    }

    private async Task UpdateRegionsAsync(Data.StoryEpic epic, List<StoryEpicRegionDto> regionDtos, CancellationToken ct)
    {
        // Remove regions that are not in DTO
        var existingRegionIds = regionDtos.Select(r => r.Id).ToHashSet();
        var regionsToRemove = epic.Regions.Where(r => !existingRegionIds.Contains(r.RegionId)).ToList();
        foreach (var region in regionsToRemove)
        {
            epic.Regions.Remove(region);
            _context.StoryEpicRegions.Remove(region);
        }

        // Update or add regions
        foreach (var regionDto in regionDtos)
        {
            var region = epic.Regions.FirstOrDefault(r => r.RegionId == regionDto.Id);
            if (region == null)
            {
                region = new Data.StoryEpicRegion
                {
                    EpicId = epic.Id,
                    RegionId = regionDto.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                epic.Regions.Add(region);
            }

            region.Label = regionDto.Label;
            region.ImageUrl = regionDto.ImageUrl;
            region.SortOrder = regionDto.SortOrder;
            region.IsLocked = regionDto.IsLocked;
            region.X = regionDto.X;
            region.Y = regionDto.Y;
            region.UpdatedAt = DateTime.UtcNow;
        }
    }

    private async Task UpdateStoryNodesAsync(Data.StoryEpic epic, List<StoryEpicStoryNodeDto> storyNodeDtos, CancellationToken ct)
    {
        // Remove story nodes that are not in DTO
        var existingStoryKeys = storyNodeDtos.Select(s => (s.StoryId, s.RegionId)).ToHashSet();
        var nodesToRemove = epic.StoryNodes
            .Where(sn => !existingStoryKeys.Contains((sn.StoryId, sn.RegionId)))
            .ToList();
        foreach (var node in nodesToRemove)
        {
            epic.StoryNodes.Remove(node);
            _context.StoryEpicStoryNodes.Remove(node);
        }

        // Update or add story nodes
        foreach (var storyNodeDto in storyNodeDtos)
        {
            var storyNode = epic.StoryNodes
                .FirstOrDefault(sn => sn.StoryId == storyNodeDto.StoryId && sn.RegionId == storyNodeDto.RegionId);
            
            if (storyNode == null)
            {
                storyNode = new Data.StoryEpicStoryNode
                {
                    EpicId = epic.Id,
                    StoryId = storyNodeDto.StoryId,
                    RegionId = storyNodeDto.RegionId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                epic.StoryNodes.Add(storyNode);
            }

            storyNode.RewardImageUrl = storyNodeDto.RewardImageUrl;
            storyNode.SortOrder = storyNodeDto.SortOrder;
            storyNode.X = storyNodeDto.X;
            storyNode.Y = storyNodeDto.Y;
            storyNode.UpdatedAt = DateTime.UtcNow;
        }
    }

    private async Task UpdateUnlockRulesAsync(Data.StoryEpic epic, List<StoryEpicUnlockRuleDto> ruleDtos, CancellationToken ct)
    {
        // Remove rules that are not in DTO (simplified - in production might want to keep IDs)
        var rulesToRemove = epic.UnlockRules.ToList();
        foreach (var rule in rulesToRemove)
        {
            epic.UnlockRules.Remove(rule);
            _context.StoryEpicUnlockRules.Remove(rule);
        }

        // Add all rules from DTO
        foreach (var ruleDto in ruleDtos)
        {
            var rule = new Data.StoryEpicUnlockRule
            {
                EpicId = epic.Id,
                Type = ruleDto.Type,
                FromId = ruleDto.FromId,
                ToRegionId = ruleDto.ToRegionId,
                RequiredStoriesCsv = ruleDto.RequiredStories.Any() 
                    ? string.Join(",", ruleDto.RequiredStories) 
                    : null,
                MinCount = ruleDto.MinCount,
                StoryId = ruleDto.StoryId,
                SortOrder = ruleDto.SortOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            epic.UnlockRules.Add(rule);
        }
    }

    private StoryEpicPreviewDto GeneratePreview(StoryEpicDto epic)
    {
        var nodes = new List<StoryEpicPreviewNodeDto>();
        var edges = new List<StoryEpicPreviewEdgeDto>();

        // Add region nodes
        foreach (var region in epic.Regions)
        {
            nodes.Add(new StoryEpicPreviewNodeDto
            {
                Id = region.Id,
                Type = "region",
                Label = region.Label,
                ImageUrl = region.ImageUrl,
                X = region.X,
                Y = region.Y
            });
        }

        // Add story nodes
        foreach (var story in epic.Stories)
        {
            nodes.Add(new StoryEpicPreviewNodeDto
            {
                Id = story.StoryId,
                Type = "story",
                Label = story.StoryId, // Could be enhanced with story title
                ImageUrl = story.RewardImageUrl,
                X = story.X,
                Y = story.Y
            });

            // Add edge from region to story
            edges.Add(new StoryEpicPreviewEdgeDto
            {
                FromId = story.RegionId,
                ToId = story.StoryId,
                Type = "contains"
            });
        }

        // Add unlock rule edges
        foreach (var rule in epic.Rules)
        {
            edges.Add(new StoryEpicPreviewEdgeDto
            {
                FromId = rule.FromId,
                ToId = rule.ToRegionId,
                Type = "unlock"
            });
        }

        return new StoryEpicPreviewDto
        {
            Nodes = nodes,
            Edges = edges
        };
    }
}

