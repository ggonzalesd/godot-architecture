using StructProject.Core.Entities.Models;

namespace StructProject.Core.Entities.Repository;

public interface IPlayerRepository
{
  Task<Player> GetPlayerByIdAsync(int playerId);
  Task<IEnumerable<Player>> GetAllPlayersAsync();
  Task AddPlayerAsync(Player player);
  Task UpdatePlayerAsync(Player player);
  Task DeletePlayerAsync(int playerId);
}
