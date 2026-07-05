using Microsoft.EntityFrameworkCore;
using StructProject.Database.Entities;

namespace StructProject.Database.Context;

public class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options)
{
  public DbSet<PlayerDB> Players => Set<PlayerDB>();
}
