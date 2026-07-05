using Godot;
using StructProject.Core.Entities.Logic;
using StructProject.GodotPresentation.Scripts.Containers;
using StructProject.GodotPresentation.Scripts.Adapters;
using CorePlayer = StructProject.Core.Entities.Models.Player;
using CorePosition = StructProject.Core.Shared.Models.Vec2;

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
      GetPosition: () => new CorePosition(
        X: Position.X,
        Y: Position.Y
      ),
      GetVelocity: () => new CorePosition(
        X: LinearVelocity.X,
        Y: LinearVelocity.Y
      ),
      ApplyForce: (force) => ApplyCentralForce(
        force: new Vector2(force.X, force.Y)
      ),
      ApplyVelocity: (velocity) => LinearVelocity = new Vector2(
        x: velocity.X,
        y: velocity.Y
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
    _playerLoopLogic.Update(delta);

    CannotPivot.Rotate(Mathf.DegToRad(speed * (float)delta));
  }
}