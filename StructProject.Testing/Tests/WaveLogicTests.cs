using System.Collections.Generic;
using StructProject.Core.Entities.Enemies;
using StructProject.Core.Entities.Waves;
using StructProject.Core.Logic.Waves;
using StructProject.Core.Logic.Pickups;
using StructProject.Core.Shared.Service;

namespace StructProject.Testing.Tests;

public static class WaveLogicTests
{
  private static ILogger TestLogger() => new StubLogger();

  private class StubLogger : ILogger
  {
    public void Log(params object[] message) { }
  }

  private static List<EnemySpawnEvent> CaptureSpawns(WaveControllerLogic c)
  {
    var list = new List<EnemySpawnEvent>();
    c.OnEnemySpawn += e => list.Add(e);
    return list;
  }

  public static System.Action<string>? RollsCorrectSpawnCount()
  {
    var logger = TestLogger();
    var c = new WaveControllerLogic(logger);
    var enemy = new EnemyKindSnapshot(EnemyKind.Grunt, "Grunt", 30, 80, 10, 10, 1, false, 0, 0, 0, 0);
    var ws = new WaveSpawn(enemy, Delay: 0, Count: 5, Interval: 0.1f, LaneY: 0, Direction: -1, HealthMultiplier: 1);
    var wave = new WaveSnapshot("T", 1.0f, 1.0f, 1f, new List<WaveSpawn> { ws });
    var seq = new WaveSequenceSnapshot("S", new List<WaveSnapshot> { wave }, false, 1f);
    c.Start(seq);
    var captured = CaptureSpawns(c);

    for (var i = 0; i < 100; i++) c.Update(0.05);
    if (captured.Count != 5) return msg => msg = $"expected 5 spawns, got {captured.Count}";
    return null;
  }

  public static System.Action<string>? AdvancesAcrossWaves()
  {
    var logger = TestLogger();
    var c = new WaveControllerLogic(logger);
    var enemy = new EnemyKindSnapshot(EnemyKind.Grunt, "Grunt", 30, 80, 10, 10, 1, false, 0, 0, 0, 0);
    var ws = new WaveSpawn(enemy, Delay: 0, Count: 2, Interval: 0.05f, LaneY: 0, Direction: -1, HealthMultiplier: 1);
    var w1 = new WaveSnapshot("W1", 0.2f, 0.1f, 1f, new List<WaveSpawn> { ws });
    var w2 = new WaveSnapshot("W2", 0.2f, 0.1f, 1f, new List<WaveSpawn> { ws });
    var seq = new WaveSequenceSnapshot("S", new List<WaveSnapshot> { w1, w2 }, false, 1f);
    c.Start(seq);

    var started = new List<int>();
    c.OnWaveStarted += (idx, _) => started.Add(idx);

    for (var i = 0; i < 60; i++) c.Update(0.05);

    if (started.Count < 2) return msg => msg = $"expected at least 2 starts, got {started.Count}";
    return null;
  }

  public static System.Action<string>? LoopsWhenConfigured()
  {
    var logger = TestLogger();
    var c = new WaveControllerLogic(logger);
    var enemy = new EnemyKindSnapshot(EnemyKind.Grunt, "Grunt", 30, 80, 10, 10, 1, false, 0, 0, 0, 0);
    var ws = new WaveSpawn(enemy, Delay: 0, Count: 1, Interval: 0.05f, LaneY: 0, Direction: -1, HealthMultiplier: 1);
    var w1 = new WaveSnapshot("W1", 0.1f, 0.0f, 1f, new List<WaveSpawn> { ws });
    var seq = new WaveSequenceSnapshot("S", new List<WaveSnapshot> { w1 }, true, 1f);
    c.Start(seq);
    for (var i = 0; i < 120; i++) c.Update(0.05);
    if (c.LoopsCompleted < 1) return msg => msg = $"expected at least one loop, got {c.LoopsCompleted}";
    return null;
  }
}
