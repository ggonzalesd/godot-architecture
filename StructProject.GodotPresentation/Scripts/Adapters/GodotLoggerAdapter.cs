using Godot;
using StructProject.Core.Shared.Service;

namespace StructProject.GodotPresentation.Scripts.Adapters;

public class GodotLoggerAdapter : ILogger
{
  public void Log(params object[] message)
  {
    GD.Print(message);
  }

  public void Error(params object[] message)
  {
    GD.PushError(message);
  }
}
