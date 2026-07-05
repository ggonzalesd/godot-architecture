using Godot;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class WaveData : Resource
{
  [Export]
  public string DisplayName { get; set; } = "Wave";

  [Export(PropertyHint.Range, "0.1,2,0.05")]
  public float DifficultyMultiplier { get; set; } = 1f;

  [Export(PropertyHint.Range, "5,600,1")]
  public float Duration { get; set; } = 30f;

  [Export]
  public Godot.Collections.Array<EnemySpawnConfig> Spawns { get; set; } = [];

  [Export(PropertyHint.Range, "0,600,1")]
  public float TimeBeforeNext { get; set; } = 4f;
}
