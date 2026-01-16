using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class TreeOfHeroesConfigCraftService : ITreeOfHeroesConfigCraftService
{
    private readonly ITreeOfHeroesConfigCraftRepository _repository;
    private readonly XooDbContext _db;
    private readonly ILogger<TreeOfHeroesConfigCraftService> _logger;

    public TreeOfHeroesConfigCraftService(
        ITreeOfHeroesConfigCraftRepository repository,
        XooDbContext db,
        ILogger<TreeOfHeroesConfigCraftService> logger)
    {
        _repository = repository;
        _db = db;
        _logger = logger;
    }

    public async Task<TreeOfHeroesConfigCraftDto> GetCraftAsync(Guid configId, CancellationToken ct = default)
    {
        var config = await _repository.GetWithDetailsAsync(configId, ct);
        if (config == null)
            throw new KeyNotFoundException($"TreeOfHeroesConfigCraft with Id '{configId}' not found");

        return MapToDto(config);
    }

    public async Task<ListTreeOfHeroesConfigCraftsResponse> ListCraftsAsync(string? status = null, CancellationToken ct = default)
    {
        var configs = await _repository.ListAsync(status, ct);
        var totalCount = await _repository.CountAsync(status, ct);

        var items = configs.Select(c => new TreeOfHeroesConfigListItemDto
        {
            Id = c.Id,
            Label = c.Label,
            Status = c.Status,
            UpdatedAt = c.UpdatedAt,
            CreatedByUserId = c.CreatedByUserId
        }).ToList();

        return new ListTreeOfHeroesConfigCraftsResponse
        {
            Configs = items,
            TotalCount = totalCount
        };
    }

    public async Task<ListTreeOfHeroesConfigDefinitionsResponse> ListDefinitionsAsync(string? status = null, CancellationToken ct = default)
    {
        var query = _db.TreeOfHeroesConfigDefinitions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status);

        var configs = await query.OrderByDescending(x => x.UpdatedAt).ToListAsync(ct);
        var items = configs.Select(c => new TreeOfHeroesConfigListItemDto
        {
            Id = c.Id,
            Label = c.Label,
            Status = c.Status,
            UpdatedAt = c.UpdatedAt,
            CreatedByUserId = null
        }).ToList();

        return new ListTreeOfHeroesConfigDefinitionsResponse
        {
            Configs = items,
            TotalCount = items.Count
        };
    }

    public async Task<TreeOfHeroesConfigCraftDto> CreateCraftAsync(Guid userId, CreateTreeOfHeroesConfigCraftRequest request, CancellationToken ct = default)
    {
        await ValidateNodesAsync(request.Nodes, request.Edges, ct);

        var config = new TreeOfHeroesConfigCraft
        {
            Id = Guid.NewGuid(),
            Label = request.Label,
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Nodes = request.Nodes.Select(n => new TreeOfHeroesConfigCraftNode
            {
                Id = n.Id == Guid.Empty ? Guid.NewGuid() : n.Id,
                HeroDefinitionId = n.HeroDefinitionId,
                PositionX = n.PositionX,
                PositionY = n.PositionY,
                CourageCost = n.CourageCost,
                CuriosityCost = n.CuriosityCost,
                ThinkingCost = n.ThinkingCost,
                CreativityCost = n.CreativityCost,
                SafetyCost = n.SafetyCost,
                IsStartup = n.IsStartup ?? false,
                PrerequisitesJson = n.Prerequisites != null && n.Prerequisites.Count > 0 
                    ? JsonSerializer.Serialize(n.Prerequisites) 
                    : "[]"
            }).ToList(),
            Edges = request.Edges.Select(e => new TreeOfHeroesConfigCraftEdge
            {
                Id = e.Id == Guid.Empty ? Guid.NewGuid() : e.Id,
                FromHeroId = e.FromHeroId,
                ToHeroId = e.ToHeroId
            }).ToList()
        };

        await _repository.CreateAsync(config, ct);
        return MapToDto(config);
    }

    public async Task<TreeOfHeroesConfigCraftDto> UpdateCraftAsync(Guid userId, Guid configId, UpdateTreeOfHeroesConfigCraftRequest request, CancellationToken ct = default)
    {
        var config = await _repository.GetWithDetailsAsync(configId, ct);
        if (config == null)
            throw new KeyNotFoundException($"TreeOfHeroesConfigCraft with Id '{configId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(config.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot update TreeOfHeroesConfigCraft in status '{currentStatus}'");

        if (config.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can update this TreeOfHeroesConfigCraft");

        if (request.Label != null) config.Label = request.Label;

        if (request.Nodes != null || request.Edges != null)
        {
            var nodes = request.Nodes ?? new List<TreeOfHeroesConfigNodeDto>();
            var edges = request.Edges ?? new List<TreeOfHeroesConfigEdgeDto>();
            await ValidateNodesAsync(nodes, edges, ct);

            _db.TreeOfHeroesConfigCraftNodes.RemoveRange(config.Nodes);
            _db.TreeOfHeroesConfigCraftEdges.RemoveRange(config.Edges);

            config.Nodes = nodes.Select(n => new TreeOfHeroesConfigCraftNode
            {
                Id = n.Id == Guid.Empty ? Guid.NewGuid() : n.Id,
                ConfigCraftId = config.Id,
                HeroDefinitionId = n.HeroDefinitionId,
                PositionX = n.PositionX,
                PositionY = n.PositionY,
                CourageCost = n.CourageCost,
                CuriosityCost = n.CuriosityCost,
                ThinkingCost = n.ThinkingCost,
                CreativityCost = n.CreativityCost,
                SafetyCost = n.SafetyCost,
                IsStartup = n.IsStartup ?? false,
                PrerequisitesJson = n.Prerequisites != null && n.Prerequisites.Count > 0 
                    ? JsonSerializer.Serialize(n.Prerequisites) 
                    : "[]"
            }).ToList();

            config.Edges = edges.Select(e => new TreeOfHeroesConfigCraftEdge
            {
                Id = e.Id == Guid.Empty ? Guid.NewGuid() : e.Id,
                ConfigCraftId = config.Id,
                FromHeroId = e.FromHeroId,
                ToHeroId = e.ToHeroId
            }).ToList();
        }

        await _repository.SaveAsync(config, ct);
        return MapToDto(config);
    }

    public async Task SubmitForReviewAsync(Guid userId, Guid configId, CancellationToken ct = default)
    {
        var config = await _repository.GetAsync(configId, ct);
        if (config == null)
            throw new KeyNotFoundException($"TreeOfHeroesConfigCraft with Id '{configId}' not found");

        if (config.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can submit this TreeOfHeroesConfigCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(config.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot submit TreeOfHeroesConfigCraft in status '{currentStatus}'");

        config.Status = AlchimaliaUniverseStatus.SentForApproval.ToDb();
        await _repository.SaveAsync(config, ct);
        _logger.LogInformation("TreeOfHeroesConfigCraft {ConfigId} submitted for review by {UserId}", configId, userId);
    }

    public async Task ReviewAsync(Guid reviewerId, Guid configId, ReviewTreeOfHeroesConfigCraftRequest request, CancellationToken ct = default)
    {
        var config = await _repository.GetAsync(configId, ct);
        if (config == null)
            throw new KeyNotFoundException($"TreeOfHeroesConfigCraft with Id '{configId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(config.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && currentStatus != AlchimaliaUniverseStatus.InReview)
            throw new InvalidOperationException($"Cannot review TreeOfHeroesConfigCraft in status '{currentStatus}'");

        config.Status = request.Approve
            ? AlchimaliaUniverseStatus.Approved.ToDb()
            : AlchimaliaUniverseStatus.ChangesRequested.ToDb();
        config.ReviewedByUserId = reviewerId;
        config.ReviewNotes = request.Notes;
        await _repository.SaveAsync(config, ct);
    }

    public async Task PublishAsync(Guid publisherId, Guid configId, CancellationToken ct = default)
    {
        var config = await _repository.GetWithDetailsAsync(configId, ct);
        if (config == null)
            throw new KeyNotFoundException($"TreeOfHeroesConfigCraft with Id '{configId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(config.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Approved)
            throw new InvalidOperationException($"Cannot publish TreeOfHeroesConfigCraft in status '{currentStatus}'. Must be Approved.");

        var definitionId = config.PublishedDefinitionId ?? config.Id;
        var definition = await _db.TreeOfHeroesConfigDefinitions
            .Include(d => d.Nodes)
            .Include(d => d.Edges)
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
        {
            definition = new TreeOfHeroesConfigDefinition
            {
                Id = definitionId,
                Label = config.Label,
                Status = AlchimaliaUniverseStatus.Published.ToDb(),
                PublishedByUserId = publisherId,
                PublishedAtUtc = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Nodes = config.Nodes.Select(n => new TreeOfHeroesConfigDefinitionNode
                {
                    Id = Guid.NewGuid(),
                    HeroDefinitionId = n.HeroDefinitionId,
                    PositionX = n.PositionX,
                    PositionY = n.PositionY,
                    CourageCost = n.CourageCost,
                    CuriosityCost = n.CuriosityCost,
                    ThinkingCost = n.ThinkingCost,
                    CreativityCost = n.CreativityCost,
                    SafetyCost = n.SafetyCost,
                    IsStartup = n.IsStartup,
                    PrerequisitesJson = n.PrerequisitesJson
                }).ToList(),
                Edges = config.Edges.Select(e => new TreeOfHeroesConfigDefinitionEdge
                {
                    Id = Guid.NewGuid(),
                    FromHeroId = e.FromHeroId,
                    ToHeroId = e.ToHeroId
                }).ToList()
            };
            _db.TreeOfHeroesConfigDefinitions.Add(definition);
        }
        else
        {
            definition.Label = config.Label;
            definition.Status = AlchimaliaUniverseStatus.Published.ToDb();
            definition.PublishedByUserId = publisherId;
            definition.PublishedAtUtc = DateTime.UtcNow;
            definition.UpdatedAt = DateTime.UtcNow;

            _db.TreeOfHeroesConfigDefinitionNodes.RemoveRange(definition.Nodes);
            _db.TreeOfHeroesConfigDefinitionEdges.RemoveRange(definition.Edges);

            definition.Nodes = config.Nodes.Select(n => new TreeOfHeroesConfigDefinitionNode
            {
                Id = Guid.NewGuid(),
                ConfigDefinitionId = definitionId,
                HeroDefinitionId = n.HeroDefinitionId,
                PositionX = n.PositionX,
                PositionY = n.PositionY,
                CourageCost = n.CourageCost,
                CuriosityCost = n.CuriosityCost,
                ThinkingCost = n.ThinkingCost,
                CreativityCost = n.CreativityCost,
                SafetyCost = n.SafetyCost,
                IsStartup = n.IsStartup,
                PrerequisitesJson = n.PrerequisitesJson
            }).ToList();

            definition.Edges = config.Edges.Select(e => new TreeOfHeroesConfigDefinitionEdge
            {
                Id = Guid.NewGuid(),
                ConfigDefinitionId = definitionId,
                FromHeroId = e.FromHeroId,
                ToHeroId = e.ToHeroId
            }).ToList();
        }

        config.PublishedDefinitionId = definitionId;
        config.Status = AlchimaliaUniverseStatus.Published.ToDb();
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("TreeOfHeroesConfigCraft {ConfigId} published by {UserId}", configId, publisherId);
    }

    private async Task ValidateNodesAsync(List<TreeOfHeroesConfigNodeDto> nodes, List<TreeOfHeroesConfigEdgeDto> edges, CancellationToken ct)
    {
        var heroIds = nodes.Select(n => n.HeroDefinitionId).Distinct().ToList();
        if (heroIds.Count == 0)
            return;

        var existingHeroIds = await _db.HeroDefinitionDefinitions
            .Where(h => heroIds.Contains(h.Id) && h.Status == AlchimaliaUniverseStatus.Published.ToDb())
            .Select(h => h.Id)
            .ToListAsync(ct);

        var missing = heroIds.Except(existingHeroIds).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException($"Tree config contains unpublished or missing heroes: {string.Join(", ", missing)}");

        if (edges.Count > 0)
        {
            var nodeSet = new HashSet<string>(heroIds);
            var invalidEdges = edges
                .Where(e => !nodeSet.Contains(e.FromHeroId) || !nodeSet.Contains(e.ToHeroId))
                .ToList();
            if (invalidEdges.Count > 0)
                throw new InvalidOperationException("Tree config contains edges that reference missing nodes.");
        }
    }

    private TreeOfHeroesConfigCraftDto MapToDto(TreeOfHeroesConfigCraft config)
    {
        return new TreeOfHeroesConfigCraftDto
        {
            Id = config.Id,
            PublishedDefinitionId = config.PublishedDefinitionId,
            Label = config.Label,
            Status = config.Status,
            CreatedByUserId = config.CreatedByUserId,
            ReviewedByUserId = config.ReviewedByUserId,
            ReviewNotes = config.ReviewNotes,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt,
            Nodes = config.Nodes.Select(n => new TreeOfHeroesConfigNodeDto
            {
                Id = n.Id,
                HeroDefinitionId = n.HeroDefinitionId,
                PositionX = n.PositionX,
                PositionY = n.PositionY,
                CourageCost = n.CourageCost,
                CuriosityCost = n.CuriosityCost,
                ThinkingCost = n.ThinkingCost,
                CreativityCost = n.CreativityCost,
                SafetyCost = n.SafetyCost,
                IsStartup = n.IsStartup,
                Prerequisites = string.IsNullOrWhiteSpace(n.PrerequisitesJson) || n.PrerequisitesJson == "[]"
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(n.PrerequisitesJson) ?? new List<string>()
            }).ToList(),
            Edges = config.Edges.Select(e => new TreeOfHeroesConfigEdgeDto
            {
                Id = e.Id,
                FromHeroId = e.FromHeroId,
                ToHeroId = e.ToHeroId
            }).ToList()
        };
    }
}
