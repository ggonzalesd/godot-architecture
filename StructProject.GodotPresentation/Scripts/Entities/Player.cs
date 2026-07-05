using Godot;
using StructProject.Core.Entities.Models;
using StructProject.GodotPresentation.Scripts.Containers;
using CorePosition = StructProject.Core.Shared.Models.Vec2;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Player : RigidBody2D
{
  [Export]
  private Node2D CannotPivot { get; set; } = null!;

  private PlayerBody _body = null!;

  public override void _Ready()
  {
    _body = new PlayerBody(
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
  }

  public override void _Process(double delta)
  {
    BaseContainer.Instance.PlayerLoopLogic.Update(
      delta: delta,
      body: _body
    );

    var aim = BaseContainer.Instance.PlayerLoopLogic.AimingDirection;
    CannotPivot.Rotation = Mathf.Atan2(aim.Y, aim.X);
  }
}