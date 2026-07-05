using Godot;
using StructProject.Core.Entities.Logic;
using StructProject.GodotPresentation.Scripts.Containers;
using StructProject.GodotPresentation.Scripts.Adapters;
using CorePlayer = StructProject.Core.Entities.Models.Player;
using CorePosition = StructProject.Core.Shared.Models.Position;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Player : RigidBody2D
{
  [Export]
  private Node2D CannotPivot { get; set; } = null!;

  [Export]
  private float speed = 100f;

  private CorePlayer _player = null!;
  private PlayerLoopLogic _playerLoopLogic = null!;

  public override void _Ready()
  {
    _player = new CorePlayer(
      name: "Player1",
      level: 1,
      health: 100,
      getPosition: () => new CorePosition(
        X: Position.X,
        Y: Position.Y
      )
    );

    _playerLoopLogic = new PlayerLoopLogic(
      Player: _player,
      Logger: BaseContainer.Instance.Logger,
      Inputs: BaseContainer.Instance.Inputs
    );
  }

  public override void _Process(double delta)
  {
    _playerLoopLogic.Update();

    CannotPivot.Rotate(Mathf.DegToRad(speed * (float)delta));
  }
}