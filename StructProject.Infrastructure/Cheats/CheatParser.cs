using System.Collections.Generic;

namespace StructProject.Infrastructure.Cheats;

public enum CheatKind
{
  God,
  Coins,
  Wave,
  Weapon,
  EnemySpeed,
  SkipMenu,
  OneShot
}

public record CheatCommand(CheatKind Kind, IReadOnlyList<string> Args);

public class CheatParser
{
  public IReadOnlyList<CheatCommand> Parse(string[] args)
  {
    var list = new List<CheatCommand>();
    for (var i = 0; i < args.Length; i++)
    {
      var a = args[i];
      CheatKind? kind = a switch
      {
        "--god" => CheatKind.God,
        "--coins" => CheatKind.Coins,
        "--wave" => CheatKind.Wave,
        "--weapon" => CheatKind.Weapon,
        "--enemy-speed" => CheatKind.EnemySpeed,
        "--skip-menu" => CheatKind.SkipMenu,
        "--oneshot" => CheatKind.OneShot,
        _ => null
      };
      if (kind == null) continue;

      var needsArg = kind is CheatKind.Coins or CheatKind.Wave or CheatKind.Weapon or CheatKind.EnemySpeed;
      var cmdArgs = new List<string>();
      if (needsArg && i + 1 < args.Length && !args[i + 1].StartsWith("--"))
      {
        cmdArgs.Add(args[i + 1]);
        i++;
      }
      list.Add(new CheatCommand(kind.Value, cmdArgs));
    }
    return list;
  }
}
