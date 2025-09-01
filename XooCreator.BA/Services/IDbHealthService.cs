namespace XooCreator.BA.Services;

public record DbHealthResult(bool CanConnect, IEnumerable<string> PendingMigrations);

public interface IDbHealthService
{
    Task<DbHealthResult> GetAsync(CancellationToken ct);
}
