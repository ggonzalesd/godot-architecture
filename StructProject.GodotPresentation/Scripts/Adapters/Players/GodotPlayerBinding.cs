using Godot;
using StructProject.Core.Entities.Player;
using StructProject.Core.Shared.Models;

namespace StructProject.GodotPresentation.Scripts.Adapters.Players;

public class GodotPlayerBinding(CharacterBody2D body, Node2D spawnPoint) : IBinding
{
  public void ApplyVelocity(Vec2 velocity)
  {
    body.Velocity = new Vector2(velocity.X, velocity.Y);
    body.MoveAndSlide();
  }

  public void ApplyAxisY(float velocityY)
  {
  }

  public void ApplyForce(Vec2 force)
  {
  }

  public bool IsOnFloor() => body.IsOnFloor();

  public Vec2 GetMuzzle()
  {
    var muzzlePosition = spawnPoint.GlobalPosition;
    return new Vec2(muzzlePosition.X, muzzlePosition.Y);
  }

  public Vec2 GetPosition()
  {
    var pos = body.GlobalPosition;
    return new Vec2(pos.X, pos.Y);
  }
}