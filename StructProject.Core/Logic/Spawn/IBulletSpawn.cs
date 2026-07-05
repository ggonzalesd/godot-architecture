using StructProject.Core.Shared.Models;

namespace StructProject.Core.Logic.Spawn;

public interface IBulletSpawn
{
  void SpawnBullet(
    Vec2 position,
    Vec2 direction,
    float speed,
    float lifetime,
    int damage = 0,
    string sourceTag = "player"
  );
}
