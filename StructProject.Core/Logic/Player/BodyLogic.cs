using StructProject.Core.Entities.Player;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Logic.Player;

public class BodyLogic(
  IInputActions Inputs
)
{
  public const float MoveSpeed = 220f;
  public const float Acceleration = 1800f;
  public const float Friction = 1600f;
  public const float JumpVelocity = -460f;
  public const float CoyoteTime = 0.12f;
  public const float JumpBuffer = 0.12f;

  public void Update(double delta, Body body, IBinding binding, JumpState jump)
  {
    var dt = (float)delta;
    var axisX = Inputs.Axis.X;

    jump.CoyoteRemaining = MathF.Max(0f, jump.CoyoteRemaining - dt);
    jump.BufferRemaining = MathF.Max(0f, jump.BufferRemaining - dt);

    var wantsJump = Inputs.JumpPressed;
    if (wantsJump)
    {
      jump.BufferRemaining = JumpBuffer;
    }

    var targetVX = axisX * MoveSpeed;

    var currentVX = body.Velocity.X;
    var newVX = MathF.Abs(targetVX) > 0.01f
      ? MoveTowards(currentVX, targetVX, Acceleration * dt)
      : MoveTowards(currentVX, 0f, Friction * dt);

    var newVY = body.Velocity.Y;

    if (binding.IsOnFloor())
    {
      jump.CoyoteRemaining = CoyoteTime;
    }

    var canJump = (binding.IsOnFloor() || jump.CoyoteRemaining > 0f) && jump.BufferRemaining > 0f;
    if (canJump)
    {
      newVY = JumpVelocity;
      jump.CoyoteRemaining = 0f;
      jump.BufferRemaining = 0f;
    }

    binding.ApplyVelocity(new Vec2(newVX, newVY));
  }

  public void UpdateAxisY(double delta, Body body, IBinding binding)
  {
    var dt = (float)delta;
    binding.ApplyAxisY(MoveTowards(body.Velocity.Y, 0f, Friction * dt));
  }

  private static float MoveTowards(float current, float target, float maxDelta)
  {
    var diff = target - current;
    if (MathF.Abs(diff) <= maxDelta)
    {
      return target;
    }
    return current + MathF.Sign(diff) * maxDelta;
  }
}
