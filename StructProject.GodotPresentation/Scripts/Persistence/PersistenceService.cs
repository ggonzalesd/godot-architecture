using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StructProject.Database.Context;
using StructProject.Database.Entities;
using StructProject.Database.Persistence;
using StructProject.Infrastructure.Persistence;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Persistence;

public class PersistenceService
{
  public PlayerProfileRepository ProfileRepo { get; }
  public ScoreRepository ScoreRepo { get; }
  public SaveSlotRepository SaveRepo { get; }

  public PersistenceService(GameDbContextFactory factory)
  {
    ProfileRepo = new PlayerProfileRepository(factory);
    ScoreRepo = new ScoreRepository(factory);
    SaveRepo = new SaveSlotRepository(factory);
  }

  public async Task AutoSaveRunAsync(int score, int wave, int coins)
  {
    var profile = await ProfileRepo.GetOrCreateAsync();
    if (score > profile.BestScore) profile.BestScore = score;
    if (wave > profile.BestWave) profile.BestWave = wave;
    profile.Coins += coins;
    await ProfileRepo.SaveAsync(profile);
  }

  public Task<int> SubmitScoreAsync(string name, int score, int wave) =>
    ScoreRepo.SubmitAsync(name, score, wave);
}
