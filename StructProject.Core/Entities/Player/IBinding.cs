using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Player;

public interface IBinding
{
  void ApplyVelocity(Vec2 velocity);
  void ApplyForce(Vec2 force);
  void ApplyAxisY(float velocityY);

  bool IsOnFloor();

  Vec2 GetMuzzle();
  Vec2 GetPosition();
}
