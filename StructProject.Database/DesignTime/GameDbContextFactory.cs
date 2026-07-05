using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StructProject.Database.Context;

namespace StructProject.Database.DesignTime;

public class GameDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
{
  public GameDbContext CreateDbContext(string[] args)
  {
    // From env
    var envPath = Environment.GetEnvironmentVariable("GAME_DB_PATH");

    // Dynamic path to the database file from args
    var savePath = args.Length > 0 ? args[0] : envPath ?? "game.db";

    var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>()
      .UseSqlite($"Data Source={savePath}")
      .Options;

    return new GameDbContext(optionsBuilder);
  }
}
