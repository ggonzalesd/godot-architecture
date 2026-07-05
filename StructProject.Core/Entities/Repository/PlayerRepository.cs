using StructProject.Core.Entities.Models;

namespace StructProject.Core.Entities.Repository;

public interface IPlayerRepository
{
  Task<PlayerBody> GetPlayerByIdAsync(int playerId);
  Task<IEnumerable<PlayerBody>> GetAllPlayersAsync();
  Task AddPlayerAsync(PlayerBody player);
  Task UpdatePlayerAsync(PlayerBody player);
  Task DeletePlayerAsync(int playerId);
}