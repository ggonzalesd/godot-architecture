using Microsoft.EntityFrameworkCore;
using StructProject.Database.Context;
using StructProject.Database.Entities;
using StructProject.Database.Persistence;
using GameContext = StructProject.Database.Context.GameDbContext;
using ContextFactory = StructProject.Database.Persistence.GameDbContextFactory;

namespace StructProject.Infrastructure.Persistence;

public class SaveSlotRepository(IDbContextFactory<GameDbContext> factory)
{
  public async Task<SaveSlotDB?> GetAsync(int slot)
  {
    await using var ctx = await factory.CreateDbContextAsync();
    return await ctx.SaveSlots.FirstOrDefaultAsync(s => s.Slot == slot);
  }

  public async Task SaveAsync(int slot, string name, string jsonState)
  {
    await using var ctx = await factory.CreateDbContextAsync();
    var existing = await ctx.SaveSlots.FirstOrDefaultAsync(s => s.Slot == slot);
    if (existing == null)
    {
      ctx.SaveSlots.Add(new SaveSlotDB { Slot = slot, Name = name, JsonState = jsonState });
    }
    else
    {
      existing.Name = name;
      existing.JsonState = jsonState;
      existing.UpdatedAt = DateTime.UtcNow;
    }
    await ctx.SaveChangesAsync();
  }
}
