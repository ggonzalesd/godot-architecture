using Godot;
using StructProject.Core.Entities.Pickups;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class PickupConfig : Resource
{
  [Export]
  public PickupKind Kind { get; set; } = PickupKind.Coin;

  [Export]
  public string DisplayName { get; set; } = "Pickup";

  [Export(PropertyHint.Range, "0,60,0.5")]
  public float Duration { get; set; } = 0f;

  [Export(PropertyHint.Range, "0,500,1")]
  public int Amount { get; set; } = 1;

  [Export]
  public Color TintColor { get; set; } = Colors.White;

  [Export]
  public Vector2 Scale { get; set; } = new Vector2(0.07f, 0.07f);

  [Export]
  public PackedScene? Scene { get; set; }

  public PickupSnapshot ToSnapshot() => new(Kind, Duration, Amount, 0);
}
