using Microsoft.EntityFrameworkCore;
using StructProject.Database.Context;

namespace StructProject.Database.Persistence;

public class GameDbContextFactory(string dbPath) : IDbContextFactory<GameDbContext>
{
  private readonly string _connectionString = $"Data Source={dbPath}";

  public GameDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<GameDbContext>()
      .UseSqlite(_connectionString)
      .Options;

    return new GameDbContext(options);
  }
}
