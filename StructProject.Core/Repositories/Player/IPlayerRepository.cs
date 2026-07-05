using StructProject.Core.Entities.Player;

namespace StructProject.Core.Repositories.Player;

public interface IPlayerRepository
{
  Task<Body> GetPlayerByIdAsync(int playerId);
  Task<IEnumerable<Body>> GetAllPlayersAsync();
  Task AddPlayerAsync(Body player);
  Task UpdatePlayerAsync(Body player);
  Task DeletePlayerAsync(int playerId);
}
