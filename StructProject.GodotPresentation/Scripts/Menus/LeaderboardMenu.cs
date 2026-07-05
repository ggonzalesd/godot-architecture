using Godot;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Menus;

public partial class LeaderboardMenu : Control
{
  [Export] private NodePath ListPath { get; set; } = "List";
  [Export] private NodePath BackButtonPath { get; set; } = "BackButton";
  [Export] public string MainMenuScenePath { get; set; } = "res://scenes/ui/main_menu.tscn";

  public override void _Ready()
  {
    GetNode<Button>(BackButtonPath).Pressed += () =>
      GetTree().ChangeSceneToFile(MainMenuScenePath);

    LoadEntries();
  }

  private void LoadEntries()
  {
    var list = GetNode<VBoxContainer>(ListPath);
    list.AddChild(new Label { Text = "Leaderboard wires up once persistence lands (S7)." });
  }
}
