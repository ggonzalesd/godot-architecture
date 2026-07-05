using Godot;
using StructProject.Core.Entities.Player;
using StructProject.Core.Shared.Models;

namespace StructProject.GodotPresentation.Scripts.Adapters.Players;

public class GodotPlayerBinding(RigidBody2D rigidBody, Node2D spawnPoint) : Binding
{
  public void ApplyForce(Vec2 force)
  {
    rigidBody.ApplyCentralForce(
      force: new Vector2(force.X, force.Y)
    );
  }

  public void ApplyVelocity(Vec2 velocity)
  {
    rigidBody.LinearVelocity = new Vector2(velocity.X, velocity.Y);
  }

  public Vec2 GetMuzzle()
  {
    var muzzlePosition = spawnPoint.GlobalPosition;
    return new Vec2(muzzlePosition.X, muzzlePosition.Y);
  }
}