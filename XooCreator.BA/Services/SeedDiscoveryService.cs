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

        // Prefer seeding from discover-bestiary.json per-locale if available
        var seeded = await TrySeedFromBestiaryJsonAsync(ct);
        if (!seeded)
        {
            var entries = Get63Combinations().Select(c => new DiscoveryItem
            {
                Id = Guid.NewGuid(),
                ArmsKey = c.Arms,
                BodyKey = c.Body,
                HeadKey = c.Head,
                Name = $"{c.Arms}-{c.Body}-{c.Head}",
                Story = string.Empty
            }).ToList();

            _db.DiscoveryItems.AddRange(entries);
            await _db.SaveChangesAsync(ct);
        }
    }

    private async Task<bool> TrySeedFromBestiaryJsonAsync(CancellationToken ct)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new List<string>();
            foreach (var lc in LanguageCodeExtensions.All())
            {
                candidates.Add(Path.Combine(baseDir, "Data", "SeedData", lc.ToFolder(), "discover-bestiary.json"));
            }
            candidates.Add(Path.Combine(baseDir, "Data", "SeedData", "discover-bestiary.json"));
            string? path = candidates.FirstOrDefault(File.Exists);
            if (path == null) return false;

            var json = await File.ReadAllTextAsync(path, ct);
            var items = System.Text.Json.JsonSerializer.Deserialize<List<BestiaryJsonItem>>(json);
            if (items == null || items.Count == 0) return false;

            static string Denormalize(string s)
            {
                if (string.Equals(s, "None", StringComparison.OrdinalIgnoreCase)) return "—";
                return s;
            }

            var entries = new List<DiscoveryItem>(items.Count);
            foreach (var i in items)
            {
                // i.Combination is like BunnyHippoGiraffe or BunnyGiraffeNone
                var parts = SplitCombination(i.Combination);
                entries.Add(new DiscoveryItem
                {
                    Id = Guid.NewGuid(),
                    ArmsKey = Denormalize(parts.Arms),
                    BodyKey = Denormalize(parts.Body),
                    HeadKey = Denormalize(parts.Head),
                    Name = i.Name ?? $"{parts.Arms}-{parts.Body}-{parts.Head}",
                    Story = i.Story ?? string.Empty
                });
            }

            await _db.DiscoveryItems.AddRangeAsync(entries, ct);
            await _db.SaveChangesAsync(ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static (string Arms, string Body, string Head) SplitCombination(string combination)
    {
        // The JSON uses concatenated words like BunnyHippoGiraffe or BunnyGiraffeNone.
        // We map tokens by checking known keys Bunny|Hippo|Giraffe|None in order Arms,Body,Head.
        var tokens = new[] { "Bunny", "Hippo", "Giraffe", "None" };
        string arms = "None", body = "None", head = "None";
        int idx = 0;
        foreach (var partTarget in new[] { 0, 1, 2 })
        {
            foreach (var t in tokens)
            {
                if (combination.AsSpan(idx).StartsWith(t, StringComparison.Ordinal))
                {
                    if (partTarget == 0) arms = t; else if (partTarget == 1) body = t; else head = t;
                    idx += t.Length;
                    break;
                }
            }
        }
        return (arms, body, head);
    }

    private sealed class BestiaryJsonItem
    {
        public string Combination { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Story { get; set; }
        public string? ImageFileName { get; set; }
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


