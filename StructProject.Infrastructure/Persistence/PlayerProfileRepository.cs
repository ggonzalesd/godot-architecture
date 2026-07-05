using Microsoft.EntityFrameworkCore;
using StructProject.Database.Context;
using StructProject.Database.Entities;

namespace StructProject.Infrastructure.Persistence;

public class PlayerProfileRepository(IDbContextFactory<GameDbContext> factory)
{
  public async Task<PlayerProfileDB> GetOrCreateAsync()
  {
    await using var ctx = await factory.CreateDbContextAsync();
    var existing = await ctx.Profiles.FirstOrDefaultAsync();
    if (existing != null) return existing;

    var created = new PlayerProfileDB
    {
      Coins = 0,
      BestWave = 0,
      BestScore = 0,
      ExtraMaxHp = 0,
      DamageLevel = 0,
      SpeedLevel = 0,
      LastPlayedAt = DateTime.UtcNow
    };
    ctx.Profiles.Add(created);
    await ctx.SaveChangesAsync();
    return created;
  }

  public async Task SaveAsync(PlayerProfileDB profile)
  {
    await using var ctx = await factory.CreateDbContextAsync();
    ctx.Profiles.Update(profile);
    profile.LastPlayedAt = DateTime.UtcNow;
    await ctx.SaveChangesAsync();
  }
}
