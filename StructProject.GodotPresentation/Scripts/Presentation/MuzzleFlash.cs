using Godot;

namespace StructProject.GodotPresentation.Scripts.Presentation;

public partial class MuzzleFlash : Node2D
{
  [Export] public float Lifetime { get; set; } = 0.08f;
  [Export] public Sprite2D? Sprite { get; set; }

  private float _age;

  public override void _Ready()
  {
    if (Sprite == null) Sprite = GetNodeOrNull<Sprite2D>("Sprite");
  }

  public void Flash(Vector2 position, float angle)
  {
    GlobalPosition = position;
    Rotation = angle;
    _age = 0f;
    Visible = true;
    Scale = Vector2.One * 0.18f;
  }

  public override void _Process(double delta)
  {
    _age += (float)delta;
    if (_age >= Lifetime)
    {
      Visible = false;
      return;
    }
    Scale = Scale.Lerp(Vector2.Zero, (float)delta * 18f);
  }
}
