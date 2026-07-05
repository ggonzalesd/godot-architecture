using System;
using Godot;
using StructProject.Core.Entities.Enemies;
using StructProject.Core.Logic.Enemies;
using StructProject.Core.Shared.Models;
using StructProject.GodotPresentation.Scripts.Containers;
using BulletEntity = StructProject.GodotPresentation.Scripts.Entities.Bullet;
using EnemyInstanceT = StructProject.Core.Entities.Enemies.EnemyInstance;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Enemy : CharacterBody2D
{
  [ExportGroup("Nodes")]
  [Export]
  private NodePath VisualPath { get; set; } = "Visual";
  [Export]
  private NodePath BodyAreaPath { get; set; } = "BodyArea";

  private Sprite2D VisualNode = null!;
  private Area2D BodyArea = null!;

  public int InstanceId { get; set; }
  public EnemyInstanceT Instance { get; set; } = null!;
  public int ContactCooldown { get; set; }

  public override void _Ready()
  {
    VisualNode = GetNode<Sprite2D>(VisualPath);
    BodyArea = GetNode<Area2D>(BodyAreaPath);
    BodyArea.AreaEntered += OnAreaEntered;
    AddToGroup("enemies");
  }

  public void Initialize(int instanceId, EnemyInstanceT instance)
  {
    InstanceId = instanceId;
    Instance = instance;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (Instance == null) return;
    if (ContactCooldown > 0) ContactCooldown--;

    var container = BaseContainer.Instance;

    var playerPos = container.PlayerBinding?.GetPosition() ?? new Vec2(GlobalPosition.X - 100, GlobalPosition.Y);
    var ai = BaseContainer.EnemyAi.Decide(Instance, playerPos);
    Instance = BaseContainer.EnemyAi.AdvanceCooldown(Instance, delta);

    var speed = Instance.Spawn.MoveSpeed;
    if (ai.Horizontal == HorizontalIntent.Left)
    {
      Velocity = new Vector2(-speed, Velocity.Y);
      VisualNode.FlipH = true;
    }
    else if (ai.Horizontal == HorizontalIntent.Right)
    {
      Velocity = new Vector2(speed, Velocity.Y);
      VisualNode.FlipH = false;
    }
    else
    {
      Velocity = new Vector2(0, Velocity.Y);
    }

    if (ai.Vertical == VerticalIntent.Jump && IsOnFloor())
    {
      Velocity = new Vector2(Velocity.X, -380f);
    }

    if (Instance.Spawn.Flies)
    {
      Velocity = new Vector2(Velocity.X, Velocity.Y > 0 ? -20f : -80f);
    }

    MoveAndSlide();

    if (ai.WantShoot)
    {
      var origin = new Vec2(GlobalPosition.X, GlobalPosition.Y - 8);
      var dirVec = playerPos - origin;
      var dist = MathF.Sqrt(dirVec.X * dirVec.X + dirVec.Y * dirVec.Y);
      var dirNormalized = dist > 0.0001f
        ? new Vec2(dirVec.X / dist, dirVec.Y / dist)
        : new Vec2(-1, 0);

      container.EnemyBulletSpawn.SpawnBullet(
        origin,
        dirNormalized,
        ai.ShootSpeed,
        ai.ProjectileLifetime,
        ai.ShootDamage,
        sourceTag: "enemy"
      );
      Instance = BaseContainer.EnemyAi.OnShotFired(Instance);
    }

    if (Instance.IsDead)
    {
      container.OnEnemyKilled(InstanceId, Instance.Spawn.ScoreValue, Instance.Spawn.CoinDrop);
      QueueFree();
    }
  }

  public void ApplyDamage(int amount)
  {
    Instance = BaseContainer.EnemyAi.ApplyDamage(Instance, amount);
    if (Instance.IsDead)
    {
      BaseContainer.Instance.OnEnemyKilled(InstanceId, Instance.Spawn.ScoreValue, Instance.Spawn.CoinDrop);
      QueueFree();
    }
  }

  private void OnAreaEntered(Area2D area)
  {
    if (area is BulletEntity bullet && bullet.SourceTag == "player")
    {
      ApplyDamage(bullet.Damage);
      bullet.QueueFree();
      return;
    }
    if (area.Name == "HurtArea" && ContactCooldown <= 0)
    {
      BaseContainer.Instance.TryDamagePlayer(Instance.Spawn.ContactDamage);
      ContactCooldown = 30;
    }
  }
}
