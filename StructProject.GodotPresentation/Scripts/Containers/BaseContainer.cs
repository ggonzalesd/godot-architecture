using Godot;
using Microsoft.EntityFrameworkCore;
using StructProject.Core.Logic.Player;
using StructProject.Core.Shared.Service;
using StructProject.Database.Context;
using StructProject.Database.Persistence;
using StructProject.GodotPresentation.Scripts.Adapters;

namespace StructProject.GodotPresentation.Scripts.Containers;

public partial class BaseContainer : Node
{
  public ILogger Logger { get; private set; } = null!;
  public IInputActions Inputs { get; private set; } = null!;
  public BodyLogic PlayerBodyLogic { get; private set; } = null!;
  public ShootingLogic Shooting { get; private set; } = null!;
  public IDbContextFactory<GameDbContext> DbContextFactory { get; private set; } = null!;

  private static BaseContainer? _instance;

  public static BaseContainer Instance
  {
    get
    {
      if (_instance == null)
        throw new System.InvalidOperationException("BaseContainer autoload not initialized.");
      return _instance;
    }
  }

  public override void _Ready()
  {
    _instance = this;

    Logger = new GodotLoggerAdapter();

    Inputs = new GodotInputActionsAdapter(
      viewport: GetViewport()
    );

    PlayerBodyLogic = new BodyLogic(
      Inputs: Inputs
    );

    Shooting = new ShootingLogic(
      Inputs: Inputs,
      Logger: Logger
    );

    var dbPath = ProjectSettings.GlobalizePath("user://game.db");
    DbContextFactory = new GameDbContextFactory(dbPath);

    using (var ctx = DbContextFactory.CreateDbContext())
    {
      ctx.Database.Migrate();
    }

    Logger.Log($"Database initialized at {dbPath}");
  }
}