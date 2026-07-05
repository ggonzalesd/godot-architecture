namespace StructProject.Core.Shared.Models;

public partial class Vec2(float X, float Y)
{
  public float X { get; set; } = X;
  public float Y { get; set; } = Y;

  public static Vec2 Zero { get; } = new(0f, 0f);
}
