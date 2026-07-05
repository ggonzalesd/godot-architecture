using Godot;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class EnemySpawnConfig : Resource
{
  [Export]
  public EnemyKindConfig? Enemy { get; set; }

  [Export(PropertyHint.Range, "0,600,0.1")]
  public float Delay { get; set; } = 1f;

  [Export(PropertyHint.Range, "1,200,1")]
  public int Count { get; set; } = 5;

  [Export(PropertyHint.Range, "0.1,10,0.1")]
  public float Interval { get; set; } = 0.8f;

  [Export(PropertyHint.Range, "-300,300,1")]
  public float LaneY { get; set; } = 0f;

  [Export(PropertyHint.Range, "-1,1,1")]
  public int Direction { get; set; } = 1;

  [Export(PropertyHint.Range, "0.5,3,0.1")]
  public float HealthMultiplier { get; set; } = 1f;
}
