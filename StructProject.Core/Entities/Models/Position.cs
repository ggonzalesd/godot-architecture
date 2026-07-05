namespace StructProject.Core.Entities.Models;

public record Position(float X, float Y)
{
  public static Position Zero { get; } = new(0f, 0f);
}