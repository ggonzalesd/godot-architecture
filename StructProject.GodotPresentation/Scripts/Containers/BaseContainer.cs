using StructProject.Core.Shared.Service;
using StructProject.GodotPresentation.Scripts.Adapters;

namespace StructProject.GodotPresentation.Scripts.Containers;

public class BaseContainer
{
  public required ILogger Logger;

  private static BaseContainer? _instance;
  public static BaseContainer Instance
  {
    get
    {
      if (_instance != null)
        return _instance;

      var Logger = new GodotLoggerAdapter();

      _instance = new BaseContainer
      {
        Logger = Logger
      };

      return _instance;
    }
  }
}
