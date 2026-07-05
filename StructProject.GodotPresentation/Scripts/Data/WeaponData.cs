namespace StructProject.GodotPresentation.Scripts.Data;

using Godot;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class WeaponData : Resource
{
  [Export]
  public Godot.Collections.Dictionary<string, string> CustomProperties { get; set; } = [];

  [Export]
  public WeaponData? BaseWeapon { get; set; } = null;

  [Export]
  public string Name { get; set; } = "New Weapon";

  [Export]
  public int Damage { get; set; } = 10;

  [Export]
  public float Cooldown { get; set; } = 1.0f;

  [Export]
  public Texture2D? Icon { get; set; } = null;

  [Export]
  public Vector2 Size { get; set; } = new Vector2(1, 1);

  [Export]
  public Color Color { get; set; } = Colors.White;

  [Export]
  public Godot.Collections.Array<string> Tags { get; set; } = [];

  [Export]
  public Curve2D? TrajectoryCurve { get; set; } = null;

  [Export]
  public Curve? DamageFalloffCurve { get; set; } = null;

  [Export]
  public ShaderMaterial? Material { get; set; } = null;

  [Export]
  public PackedScene? ProjectileScene { get; set; } = null;
}
