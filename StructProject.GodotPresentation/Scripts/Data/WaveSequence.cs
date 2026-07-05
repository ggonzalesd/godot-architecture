using Godot;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class WaveSequence : Resource
{
  [Export]
  public string DisplayName { get; set; } = "Default Sequence";

  [Export]
  public Godot.Collections.Array<WaveData> Waves { get; set; } = [];

  [Export]
  public bool LoopOnFinish { get; set; } = true;

  [Export(PropertyHint.Range, "1,3,0.05")]
  public float LoopGrowthFactor { get; set; } = 1.15f;
}
