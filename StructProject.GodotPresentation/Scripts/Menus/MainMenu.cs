using Godot;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Menus;

public partial class MainMenu : Control
{
  [Export] private NodePath PlayButtonPath { get; set; } = "VBox/Play";
  [Export] private NodePath LeaderboardButtonPath { get; set; } = "VBox/Leaderboard";
  [Export] private NodePath OptionsButtonPath { get; set; } = "VBox/Options";
  [Export] private NodePath QuitButtonPath { get; set; } = "VBox/Quit";

  [Export] public string GameplayScenePath { get; set; } = "res://scenes/main.tscn";
  [Export] public string LeaderboardScenePath { get; set; } = "res://scenes/ui/leaderboard_menu.tscn";
  [Export] public string OptionsScenePath { get; set; } = "res://scenes/ui/options_menu.tscn";

  public override void _Ready()
  {
    var play = GetNode<Button>(PlayButtonPath);
    play.Pressed += () => GetTree().ChangeSceneToFile(GameplayScenePath);

    GetNode<Button>(LeaderboardButtonPath).Pressed += () =>
      GetTree().ChangeSceneToFile(LeaderboardScenePath);

    GetNode<Button>(OptionsButtonPath).Pressed += () =>
      GetTree().ChangeSceneToFile(OptionsScenePath);

    GetNode<Button>(QuitButtonPath).Pressed += () => GetTree().Quit();
  }
}
