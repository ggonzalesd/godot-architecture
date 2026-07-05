namespace StructProject.GodotPresentation.Scripts.Entities;

using Godot;
using System;
using StructProject.GodotPresentation.Scripts.Containers;

public partial class Bullet : Area2D
{
  [Export]
  private Node2D SpriteNode { get; set; } = null!;

  public Vector2 Velocity { get; set; }
  public float LifetimeRemaining { get; set; } = 3f;
  public int Damage { get; set; } = 10;
  public string SourceTag { get; set; } = "player";

  public override void _Ready()
  {
    AreaEntered += OnAreaEntered;
  }

  public override void _PhysicsProcess(double delta)
  {
    var dt = (float)delta;
    Position += Velocity * dt;
    LifetimeRemaining -= dt;

    if (LifetimeRemaining <= 0f)
    {
      QueueFree();
      return;
    }

    SpriteNode.GlobalRotation = Velocity.Angle();
  }

  private void OnAreaEntered(Area2D area)
  {
    if (area == this) return;
    var parent = area.GetParent();

    if (SourceTag == "player" && parent is Enemy enemy)
    {
      enemy.ApplyDamage(Damage);
      QueueFree();
      return;
    }

    if (SourceTag == "enemy" && parent is Player)
    {
      BaseContainer.Instance.TryDamagePlayer(Damage);
      QueueFree();
      return;
    }
  }
}
