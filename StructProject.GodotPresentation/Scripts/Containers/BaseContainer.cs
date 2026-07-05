using System;
using Godot;
using StructProject.Core.Shared.Service;
using StructProject.GodotPresentation.Scripts.Adapters;

namespace StructProject.GodotPresentation.Scripts.Containers;

public partial class BaseContainer : Node
{
  public required ILogger Logger;
  public required IInputActions Inputs;

  public override void _Ready()
  {
    if (_instance != null)
      QueueFree(); // If an instance already exists, free this one to ensure only one instance is active.

    Logger = new GodotLoggerAdapter();
    Inputs = new GodotInputActionsAdapter(
      viewport: GetViewport()
    );

    _instance = this; // Set the static instance to this instance.
  }


  private static BaseContainer? _instance;
  public static BaseContainer Instance
  {
    get
    {
      if (_instance == null)
      {
        GD.PrintErr("BaseContainer instance is not initialized. Ensure that a BaseContainer node is present in the scene tree.");
        throw new InvalidOperationException("BaseContainer instance is not initialized. Ensure that a BaseContainer node is present in the scene tree.");
      }

      return _instance;
    }
  }
}
