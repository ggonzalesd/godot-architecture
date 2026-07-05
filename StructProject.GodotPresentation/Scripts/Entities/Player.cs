using Godot;
using StructProject.Core.Entities.Logic;
using StructProject.GodotPresentation.Scripts.Containers;
using StructProject.GodotPresentation.Scripts.Adapters;
using CorePlayer = StructProject.Core.Entities.Models.Player;
using CorePosition = StructProject.Core.Entities.Models.Position;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Player : RigidBody2D
{
  [Export]
  private Node2D CannotPivot { get; set; } = null!;

  [Export]
  private float speed = 100f;

  private readonly CorePlayer _player;
  private PlayerLoopLogic _playerLoopLogic = null!;

  public Player()
  {
    _player = new CorePlayer("Hero", 1, 100)
    {
      Position = new CorePosition(0f, 0f),
    };
  }

  public override void _Ready()
  {
    var inputs = new GodotInputActionsAdapter();

    _playerLoopLogic = new PlayerLoopLogic
    {
      Inputs = inputs,
      Logger = BaseContainer.Instance.Logger,
      Player = _player,
    };

    Position = new Vector2(_player.Position.X, _player.Position.Y);
  }

  public override void _Process(double delta)
  {
    _playerLoopLogic.Update();

    CannotPivot.Rotate(Mathf.DegToRad(speed * (float)delta));

    _player.Position = new CorePosition(Position.X, Position.Y);
    Position = new Vector2(_player.Position.X, _player.Position.Y);
  }
}