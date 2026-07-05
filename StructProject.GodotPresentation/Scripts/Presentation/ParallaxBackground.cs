using Godot;
using StructProject.GodotPresentation.Scripts.Containers;
using StructProject.GodotPresentation.Scripts.Entities;

namespace StructProject.GodotPresentation.Scripts.Presentation;

public partial class ParallaxBackground : Node2D
{
  [Export] public NodePath SkyPath { get; set; } = "Sky";
  [Export] public NodePath MountainsPath { get; set; } = "Mountains";
  [Export] public NodePath TreesPath { get; set; } = "Trees";

  private Sprite2D _sky = null!;
  private Sprite2D _mountains = null!;
  private Sprite2D _trees = null!;
  private Node2D? _camera;

  public override void _Ready()
  {
    _sky = GetNode<Sprite2D>(SkyPath);
    _mountains = GetNode<Sprite2D>(MountainsPath);
    _trees = GetNode<Sprite2D>(TreesPath);
  }

  public override void _Process(double delta)
  {
    var camera = ResolveCamera();
    if (camera == null) return;
    var camX = camera.GlobalPosition.X;
    _sky.GlobalPosition = new Vector2(camX * 0.1f, _sky.GlobalPosition.Y);
    _mountains.GlobalPosition = new Vector2(camX * 0.35f, _mountains.GlobalPosition.Y);
    _trees.GlobalPosition = new Vector2(camX * 0.7f, _trees.GlobalPosition.Y);
  }

  private Node2D? ResolveCamera()
  {
    if (_camera != null && IsInstanceValid(_camera)) return _camera;
    var tree = GetTree();
    foreach (var node in tree.GetNodesInGroup("player_camera"))
    {
      if (node is Node2D n) { _camera = n; return n; }
    }
    var player = tree.GetFirstNodeInGroup("player_root");
    if (player is Player p)
    {
      _camera = p.GetNodeOrNull<Camera2D>("Camera");
      return _camera;
    }
    return null;
  }
}
