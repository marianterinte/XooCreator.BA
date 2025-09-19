using XooCreator.BA.Data;
using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Services;

public interface ISeedDiscoveryService
{
    Task EnsureSeedAsync(CancellationToken ct = default);
}

public sealed class SeedDiscoveryService : ISeedDiscoveryService
{
    private readonly XooDbContext _db;

    public SeedDiscoveryService(XooDbContext db)
    {
        _db = db;
    }

    public async Task EnsureSeedAsync(CancellationToken ct = default)
    {
        if (await _db.DiscoveryItems.AnyAsync(ct)) return;

        var entries = Get63Combinations().Select(c => new DiscoveryItem
        {
            Id = Guid.NewGuid(),
            ArmsKey = c.Arms,
            BodyKey = c.Body,
            HeadKey = c.Head,
            Name = $"{c.Arms}-{c.Body}-{c.Head}"
        }).ToList();

        _db.DiscoveryItems.AddRange(entries);
        await _db.SaveChangesAsync(ct);
    }

    private static IEnumerable<(string Arms, string Body, string Head)> Get63Combinations()
    {
        // Updated list from CSV provided (excludes single-body-part combos). Use "—" where empty.
        var lines = new (string Arms, string Body, string Head)[]
        {
            ("Bunny","Giraffe","—"),("Bunny","Hippo","—"),("Giraffe","Bunny","—"),("Giraffe","Hippo","—"),("Hippo","Bunny","—"),("Hippo","Giraffe","—"),
            ("Bunny","—","Giraffe"),("Bunny","—","Hippo"),("Giraffe","—","Bunny"),("Giraffe","—","Hippo"),("Hippo","—","Bunny"),("Hippo","—","Giraffe"),
            ("—","Bunny","Giraffe"),("—","Bunny","Hippo"),("—","Giraffe","Bunny"),("—","Giraffe","Hippo"),("—","Hippo","Bunny"),("—","Hippo","Giraffe"),
            ("Bunny","Bunny","Giraffe"),("Bunny","Bunny","Hippo"),("Bunny","Giraffe","Bunny"),("Bunny","Giraffe","Giraffe"),("Bunny","Giraffe","Hippo"),
            ("Bunny","Hippo","Bunny"),("Bunny","Hippo","Giraffe"),("Bunny","Hippo","Hippo"),
            ("Giraffe","Bunny","Bunny"),("Giraffe","Bunny","Giraffe"),("Giraffe","Bunny","Hippo"),("Giraffe","Giraffe","Bunny"),("Giraffe","Giraffe","Hippo"),
            ("Giraffe","Hippo","Bunny"),("Giraffe","Hippo","Giraffe"),("Giraffe","Hippo","Hippo"),
            ("Hippo","Bunny","Bunny"),("Hippo","Bunny","Giraffe"),("Hippo","Bunny","Hippo"),("Hippo","Giraffe","Bunny"),("Hippo","Giraffe","Giraffe"),
            ("Hippo","Giraffe","Hippo"),("Hippo","Hippo","Bunny"),("Hippo","Hippo","Giraffe")
        };
        return lines;
    }

    // Image path is constructed on the fly in endpoint, no need to store
}


