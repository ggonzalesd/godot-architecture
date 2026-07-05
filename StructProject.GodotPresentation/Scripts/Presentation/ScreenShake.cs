using System;
using Godot;

namespace StructProject.GodotPresentation.Scripts.Presentation;

public partial class ScreenShake : Node2D
{
  [Export] public float Decay { get; set; } = 8f;
  [Export] public float MaxAmplitude { get; set; } = 12f;

  private float _trauma;
  private Vector2 _origin;
  private RandomNumberGenerator _rng = new();

  public override void _Ready()
  {
    _origin = GlobalPosition;
  }

  public override void _Process(double delta)
  {
    var dt = (float)delta;
    _trauma = MathF.Max(0f, _trauma - dt * Decay);
    if (_trauma <= 0f)
    {
      GlobalPosition = _origin;
      return;
    }
    var t = _trauma * _trauma;
    GlobalPosition = _origin + new Vector2(
      _rng.RandfRange(-1f, 1f) * MaxAmplitude * t,
      _rng.RandfRange(-1f, 1f) * MaxAmplitude * t
    );
  }

  public void AddTrauma(float amount)
  {
    _trauma = System.Math.Clamp(_trauma + amount, 0f, 1f);
  }
}
