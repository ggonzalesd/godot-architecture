using Godot;
using StructProject.Core.Entities.Pickups;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class DropEntryResource : Resource
{
  [Export]
  public PickupKind Kind { get; set; } = PickupKind.Coin;

  [Export(PropertyHint.Range, "0,10,0.05")]
  public float Weight { get; set; } = 1f;

  public DropEntry ToCore() => new(Kind, Weight);
}
