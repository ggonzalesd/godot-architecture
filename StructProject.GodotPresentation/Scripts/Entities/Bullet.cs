namespace StructProject.GodotPresentation.Scripts.Entities;

using Godot;
using System;

public partial class Bullet : RigidBody2D
{
  [Export]
  private Node2D SpriteNode { get; set; } = null!;

  public override void _Ready()
  {
  }

  public override void _Process(double delta)
  {
    SpriteNode.GlobalRotation = LinearVelocity.Angle();
  }
}
