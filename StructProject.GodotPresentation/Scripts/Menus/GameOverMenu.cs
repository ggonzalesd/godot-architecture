using Godot;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Menus;

public partial class GameOverMenu : Control
{
  [Export] private NodePath ScoreLabelPath { get; set; } = "VBox/ScoreLabel";
  [Export] private NodePath WaveLabelPath { get; set; } = "VBox/WaveLabel";
  [Export] private NodePath CoinsLabelPath { get; set; } = "VBox/CoinsLabel";
  [Export] private NodePath RetryButtonPath { get; set; } = "VBox/Retry";
  [Export] private NodePath MainMenuButtonPath { get; set; } = "VBox/MainMenu";

  [Export] public string GameplayScenePath { get; set; } = "res://scenes/main.tscn";
  [Export] public string MainMenuScenePath { get; set; } = "res://scenes/ui/main_menu.tscn";

  public override void _Ready()
  {
    var c = BaseContainer.Instance;
    var scoreL = GetNode<Label>(ScoreLabelPath);
    var waveL = GetNode<Label>(WaveLabelPath);
    var coinsL = GetNode<Label>(CoinsLabelPath);

    scoreL.Text = $"Score: {c.Score.CurrentScore}";
    coinsL.Text = $"Coins: {c.Inventory.Coins}";
    waveL.Text = $"Reached wave {c.WaveController.CurrentWaveIndex + 1}";

    GetNode<Button>(RetryButtonPath).Pressed += () =>
    {
      GetTree().Paused = false;
      GetTree().ChangeSceneToFile(GameplayScenePath);
    };

    GetNode<Button>(MainMenuButtonPath).Pressed += () =>
    {
      GetTree().Paused = false;
      GetTree().ChangeSceneToFile(MainMenuScenePath);
    };
  }

  public override void _EnterTree()
  {
    ProcessMode = ProcessModeEnum.Always;
  }
}
