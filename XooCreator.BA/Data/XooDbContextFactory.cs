using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XooCreator.BA.Data;

public class XooDbContextFactory : IDesignTimeDbContextFactory<XooDbContext>
{
    public XooDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<XooDbContext>();
        // Fallback connection string for design-time
        var cs = Environment.GetEnvironmentVariable("XOO_DB_CS")
                 ?? "Host=localhost;Port=5432;Database=xoo_db;Username=postgres;Password=postgres;";
        optionsBuilder.UseNpgsql(cs);
        return new XooDbContext(optionsBuilder.Options);
    }
}
