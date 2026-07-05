using Godot;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Menus;

public partial class PlayLoopNode : Node
{
  [Export] public PackedScene PauseScene { get; set; } = null!;
  [Export] public PackedScene GameOverScene { get; set; } = null!;

  public override void _Ready()
  {
    CallDeferred(MethodName.Initialize);
  }

  private void Initialize()
  {
    BaseContainer.Instance.OnPlayerDied += ShowGameOver;
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event.IsActionPressed("pause_game"))
    {
      if (PauseScene == null) return;
      var tree = GetTree();
      if (tree.Paused) return;
      tree.Paused = true;
      var pause = PauseScene.Instantiate();
      GetTree().Root.AddChild(pause);
    }
  }

  private void ShowGameOver()
  {
    if (GameOverScene == null) return;
    GetTree().Paused = true;
    var go = GameOverScene.Instantiate();
    GetTree().Root.AddChild(go);
  }
}
