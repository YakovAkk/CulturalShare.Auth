using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using CulturalShare.Auth.Domain.Context;

namespace Infrastructure;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>()
            .UseNpgsql();

        return new AuthDbContext(optionsBuilder.Options);
    }
}
