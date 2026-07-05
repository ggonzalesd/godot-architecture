using System.Collections.Generic;
using Godot;
using StructProject.Core.Entities.Pickups;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class DropTableConfig : Resource
{
  [Export]
  public Godot.Collections.Array<DropEntryResource> Entries { get; set; } = [];

  public DropTableSnapshot ToSnapshot()
  {
    var entries = new List<DropEntry>(Entries.Count);
    foreach (var e in Entries)
    {
      entries.Add(e.ToCore());
    }
    return new DropTableSnapshot(entries);
  }
}
