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
            Name = $"{c.Arms}-{c.Body}-{c.Head}",
            ImageV1 = BuildImageName(c, 1),
            ImageV2 = BuildImageName(c, 2),
            ImageV3 = BuildImageName(c, 3)
        }).ToList();

        _db.DiscoveryItems.AddRange(entries);
        await _db.SaveChangesAsync(ct);
    }

    private static IEnumerable<(string Arms, string Body, string Head)> Get63Combinations()
    {
        // Based on LOI.txt list (63 combos). Use "—" where empty.
        string[] A = new[] { "Bunny", "Giraffe", "Hippo", "—" };
        string[] B = new[] { "Bunny", "Giraffe", "Hippo", "—" };
        string[] H = new[] { "Bunny", "Giraffe", "Hippo", "—" };

        // Explicit list from LOI (order is not critical for seeding)
        var lines = new (string Arms, string Body, string Head)[]
        {
            ("Bunny","—","—"),("Giraffe","—","—"),("Hippo","—","—"),
            ("—","Bunny","—"),("—","Giraffe","—"),("—","Hippo","—"),
            ("—","—","Bunny"),("—","—","Giraffe"),("—","—","Hippo"),
            ("Bunny","Bunny","—"),("Bunny","Giraffe","—"),("Bunny","Hippo","—"),
            ("Giraffe","Bunny","—"),("Giraffe","Giraffe","—"),("Giraffe","Hippo","—"),
            ("Hippo","Bunny","—"),("Hippo","Giraffe","—"),("Hippo","Hippo","—"),
            ("Bunny","—","Bunny"),("Bunny","—","Giraffe"),("Bunny","—","Hippo"),
            ("Giraffe","—","Bunny"),("Giraffe","—","Giraffe"),("Giraffe","—","Hippo"),
            ("Hippo","—","Bunny"),("Hippo","—","Giraffe"),("Hippo","—","Hippo"),
            ("—","Bunny","Bunny"),("—","Bunny","Giraffe"),("—","Bunny","Hippo"),
            ("—","Giraffe","Bunny"),("—","Giraffe","Giraffe"),("—","Giraffe","Hippo"),
            ("—","Hippo","Bunny"),("—","Hippo","Giraffe"),("—","Hippo","Hippo"),
            ("Bunny","Bunny","Bunny"),("Bunny","Bunny","Giraffe"),("Bunny","Bunny","Hippo"),
            ("Bunny","Giraffe","Bunny"),("Bunny","Giraffe","Giraffe"),("Bunny","Giraffe","Hippo"),
            ("Bunny","Hippo","Bunny"),("Bunny","Hippo","Giraffe"),("Bunny","Hippo","Hippo"),
            ("Giraffe","Bunny","Bunny"),("Giraffe","Bunny","Giraffe"),("Giraffe","Bunny","Hippo"),
            ("Giraffe","Giraffe","Bunny"),("Giraffe","Giraffe","Giraffe"),("Giraffe","Giraffe","Hippo"),
            ("Giraffe","Hippo","Bunny"),("Giraffe","Hippo","Giraffe"),("Giraffe","Hippo","Hippo"),
            ("Hippo","Bunny","Bunny"),("Hippo","Bunny","Giraffe"),("Hippo","Bunny","Hippo"),
            ("Hippo","Giraffe","Bunny"),("Hippo","Giraffe","Giraffe"),("Hippo","Giraffe","Hippo"),
            ("Hippo","Hippo","Bunny"),("Hippo","Hippo","Giraffe"),("Hippo","Hippo","Hippo")
        };
        return lines;
    }

    private static string BuildImageName((string Arms, string Body, string Head) c, int variant)
    {
        string normalize(string s) => s == "—" ? "None" : s;
        var file = $"{normalize(c.Arms)}{normalize(c.Body)}{normalize(c.Head)}V{variant}.png";
        return file;
    }
}


