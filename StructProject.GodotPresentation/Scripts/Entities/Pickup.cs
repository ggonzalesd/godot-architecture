using System;
using Godot;
using StructProject.Core.Entities.Pickups;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Pickup : Area2D
{
  [Export]
  private Sprite2D Visual { get; set; } = null!;

  public PickupKind Kind { get; private set; } = PickupKind.Coin;
  public float Duration { get; private set; }
  public int Amount { get; private set; }
  public float Lifetime { get; set; } = 8f;

  private float _age;
  private float _bobPhase;

  public void Initialize(PickupKind kind, float duration, int amount)
  {
    Kind = kind;
    Duration = duration;
    Amount = amount;
  }

  public override void _Ready()
  {
    var registry = BaseContainer.Instance.PickupRegistry;
    if (registry.Get(Kind) is { } cfg)
    {
      Modulate = cfg.TintColor;
      Visual.Scale = cfg.Scale;
    }
  }

  public override void _Process(double delta)
  {
    _age += (float)delta;
    _bobPhase += (float)delta * 4f;
    Position = new Vector2(Position.X, Position.Y + Mathf.Sin(_bobPhase) * 0.4f);

    if (_age > Lifetime) QueueFree();
  }

  public override void _PhysicsProcess(double delta)
  {
    var container = BaseContainer.Instance;
    if (container.PlayerBinding == null) return;

    var pos = Position;
    var playerPos = container.PlayerBinding.GetPosition();
    var magnetRadius = container.PowerUp.ActiveMagnetRadius;
    if (magnetRadius > 0f)
    {
      var dx = playerPos.X - pos.X;
      var dy = playerPos.Y - pos.Y;
      var d = MathF.Sqrt(dx * dx + dy * dy);
      if (d > 0.001f && d < magnetRadius)
      {
        var pullSpeed = (1f - d / magnetRadius) * 600f;
        Position += new Vector2(dx / d, dy / d) * pullSpeed * (float)delta;
      }
    }
  }
}
