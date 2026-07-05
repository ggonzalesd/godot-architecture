using Microsoft.EntityFrameworkCore;
using StructProject.Database.Context;
using StructProject.Database.Entities;

namespace StructProject.Infrastructure.Persistence;

public class ScoreRepository(IDbContextFactory<GameDbContext> factory)
{
  public async Task<int> SubmitAsync(string name, int score, int wave)
  {
    await using var ctx = await factory.CreateDbContextAsync();
    var entry = new ScoreEntryDB { Name = name, Score = score, Wave = wave };
    ctx.ScoreEntries.Add(entry);
    await ctx.SaveChangesAsync();
    return entry.Id;
  }

  public async Task<List<ScoreEntryDB>> TopAsync(int n)
  {
    await using var ctx = await factory.CreateDbContextAsync();
    return await ctx.ScoreEntries
      .OrderByDescending(e => e.Score)
      .Take(n)
      .ToListAsync();
  }
}
