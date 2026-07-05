using Godot;

namespace StructProject.GodotPresentation.Scripts.Menus;

public partial class PauseMenu : Control
{
  [Export] private NodePath ResumeButtonPath { get; set; } = "VBox/Resume";
  [Export] private NodePath SaveButtonPath { get; set; } = "VBox/Save";
  [Export] private NodePath OptionsButtonPath { get; set; } = "VBox/Options";
  [Export] private NodePath MainMenuButtonPath { get; set; } = "VBox/MainMenu";
  [Export] public string GameplayScenePath { get; set; } = "res://scenes/main.tscn";
  [Export] public string MainMenuScenePath { get; set; } = "res://scenes/ui/main_menu.tscn";

  public override void _Ready()
  {
    var resume = GetNode<Button>(ResumeButtonPath);
    resume.Pressed += () =>
    {
      var tree = GetTree();
      tree.Paused = false;
      QueueFree();
    };

    GetNode<Button>(SaveButtonPath).Pressed += () =>
      GD.Print("Save not implemented yet (S7)");

    GetNode<Button>(OptionsButtonPath).Pressed += () =>
      GD.Print("Options modal not implemented yet");

    GetNode<Button>(MainMenuButtonPath).Pressed += () =>
    {
      GetTree().Paused = false;
      GetTree().ChangeSceneToFile(MainMenuScenePath);
    };
  }

  public override void _Input(InputEvent @event)
  {
    if (@event.IsActionPressed("pause_game"))
    {
      GetTree().Paused = false;
      QueueFree();
    }
  }

  public override void _EnterTree()
  {
    ProcessMode = ProcessModeEnum.Always;
  }
}
