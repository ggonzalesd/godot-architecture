namespace StructProject.Core.Shared.Models;

public partial class Vec2(float X, float Y)
{
  public float X { get; set; } = X;
  public float Y { get; set; } = Y;

  public static Vec2 Zero { get; } = new(0f, 0f);

  public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
  public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
  public static Vec2 operator *(Vec2 a, float s) => new(a.X * s, a.Y * s);
  public static Vec2 operator /(Vec2 a, float s) => new(a.X / s, a.Y / s);
}
