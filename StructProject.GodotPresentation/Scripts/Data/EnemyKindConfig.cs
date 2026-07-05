using Godot;
using StructProject.Core.Entities.Enemies;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class EnemyKindConfig : Resource
{
  [Export]
  public EnemyKind Kind { get; set; } = EnemyKind.Grunt;

  [Export]
  public string DisplayName { get; set; } = "Grunt";

  [Export(PropertyHint.Range, "1,500,1")]
  public int MaxHealth { get; set; } = 30;

  [Export(PropertyHint.Range, "20,400,1")]
  public float MoveSpeed { get; set; } = 80f;

  [Export(PropertyHint.Range, "0,100,1")]
  public int ContactDamage { get; set; } = 10;

  [Export(PropertyHint.Range, "0,1000,1")]
  public int ScoreValue { get; set; } = 10;

  [Export(PropertyHint.Range, "0,500,1")]
  public int CoinDrop { get; set; } = 1;

  [Export]
  public Color TintColor { get; set; } = Colors.White;

  [Export]
  public Vector2 Scale { get; set; } = Vector2.One;

  [Export]
  public PackedScene? Scene { get; set; }

  [Export]
  public bool Flies { get; set; } = false;

  [Export(PropertyHint.Range, "0.25,10,0.05")]
  public float ShootCooldown { get; set; } = 2f;

  [Export(PropertyHint.Range, "100,1500,10")]
  public float ShootSpeed { get; set; } = 400f;

  [Export(PropertyHint.Range, "2,80,0.5")]
  public float ShootRange { get; set; } = 18f;

  [Export(PropertyHint.Range, "1,100,1")]
  public int ShootDamage { get; set; } = 8;

  [Export]
  public DropTableConfig? DropTable { get; set; }

  public EnemyKindSnapshot ToSnapshot() => new(
    Kind,
    DisplayName,
    MaxHealth,
    MoveSpeed,
    ContactDamage,
    ScoreValue,
    CoinDrop,
    Flies,
    ShootCooldown,
    ShootSpeed,
    ShootRange,
    ShootDamage
  );
}
