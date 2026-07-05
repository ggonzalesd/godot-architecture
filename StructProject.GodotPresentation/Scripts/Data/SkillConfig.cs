using Godot;
using StructProject.Core.Entities.Skills;

namespace StructProject.GodotPresentation.Scripts.Data;

[GlobalClass]
[Icon("res://icon.svg")]
public partial class SkillConfig : Resource
{
  [Export]
  public SkillKind Kind { get; set; } = SkillKind.Dash;

  [Export]
  public string DisplayName { get; set; } = "Skill";

  [Export(PropertyHint.Range, "0,30,0.1")]
  public float Cooldown { get; set; } = 3f;

  [Export(PropertyHint.Range, "0,10,0.05")]
  public float Duration { get; set; } = 0.5f;

  [Export(PropertyHint.Range, "100,2000,10")]
  public float Power { get; set; } = 600f;

  [Export(PropertyHint.Range, "0,1000,1")]
  public int Cost { get; set; } = 0;

  [Export]
  public Color TintColor { get; set; } = Colors.White;

  [Export]
  public Texture2D? Icon { get; set; }

  public SkillSnapshot ToSnapshot() => new(Kind, DisplayName, Cooldown, Duration, Power, Cost);
}
