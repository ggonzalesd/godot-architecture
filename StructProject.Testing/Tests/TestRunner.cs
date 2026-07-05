using Godot;
using StructProject.Testing.Tests;

namespace StructProject.Testing;

public partial class TestRunner : Node
{
  public override void _Ready()
  {
    var results = new System.Collections.Generic.List<(string Name, bool Pass, string? Error)>();

    Run("WaveController.RollsCorrectSpawnCount", WaveLogicTests.RollsCorrectSpawnCount, results);
    Run("WaveController.AdvancesAcrossWaves", WaveLogicTests.AdvancesAcrossWaves, results);
    Run("WaveController.LoopsWhenConfigured", WaveLogicTests.LoopsWhenConfigured, results);
    Run("PlayerState.HandlesIFrames", PlayerStateTests.HandlesIFrames, results);
    Run("PowerUp.AppliesAndExpires", PowerUpTests.AppliesAndExpires, results);
    Run("PowerUp.MagnetAttractionFloats", PowerUpTests.MagnetAttractionFloats, results);
    Run("DropLogic.RollsWeighted", PickupTests.RollsWeighted, results);
    Run("CheatsParser.RecognisesFlags", CheatTests.RecognisesFlags, results);

    var pass = 0;
    var fail = 0;
    foreach (var r in results)
    {
      if (r.Pass) { pass++; GD.Print("PASS", r.Name); }
      else { fail++; GD.PrintErr("FAIL", r.Name, r.Error); }
    }
    GD.Print($"Tests complete: {pass} passed / {fail} failed");
    GetTree().Quit(fail == 0 ? 0 : 1);
  }

  private static void Run(string name, System.Func<System.Action<string>> test, System.Collections.Generic.List<(string, bool, string?)> results)
  {
    try
    {
      var reportFailure = test();
      if (reportFailure == null)
      {
        results.Add((name, true, null));
      }
      else
      {
        var captured = string.Empty;
        reportFailure(captured);
        results.Add((name, false, captured));
      }
    }
    catch (System.Exception ex)
    {
      results.Add((name, false, ex.Message));
    }
  }
}
