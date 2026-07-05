using System.Threading.Tasks;
using Godot;
using StructProject.Core.Entities.Skills;
using StructProject.Core.Logic.Meta;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Menus;

public partial class MetaUpgradeMenu : Control
{
  [Export] private NodePath ListPath { get; set; } = "VBox/List";
  [Export] private NodePath RetryButtonPath { get; set; } = "VBox/Retry";
  [Export] private NodePath MainMenuButtonPath { get; set; } = "VBox/MainMenu";
  [Export] private NodePath CoinsLabelPath { get; set; } = "VBox/CoinsLabel";

  [Export] public string GameplayScenePath { get; set; } = "res://scenes/main.tscn";
  [Export] public string MainMenuScenePath { get; set; } = "res://scenes/ui/main_menu.tscn";

  private MetaProgressionLogic _logic = new();

  public override void _Ready()
  {
    Build();
    GetNode<Button>(RetryButtonPath).Pressed += () =>
    {
      GetTree().Paused = false;
      GetTree().ChangeSceneToFile(GameplayScenePath);
    };
    GetNode<Button>(MainMenuButtonPath).Pressed += () =>
    {
      GetTree().Paused = false;
      GetTree().ChangeSceneToFile(MainMenuScenePath);
    };
  }

  public override void _EnterTree()
  {
    ProcessMode = ProcessModeEnum.Always;
  }

  private void Build()
  {
    var c = BaseContainer.Instance;
    var coinsL = GetNode<Label>(CoinsLabelPath);
    coinsL.Text = $"Coins: {c.Inventory.Coins}";

    var list = GetNode<VBoxContainer>(ListPath);
    foreach (var child in list.GetChildren()) child.QueueFree();

    foreach (var (_, upgrade) in MetaProgressionState.Default.Upgrades)
    {
      var row = new HBoxContainer();
      row.AddThemeConstantOverride("separation", 8);
      var label = new Label
      {
        Text = $"{upgrade.DisplayName} (Lvl {upgrade.Level}/{upgrade.MaxLevel})",
        CustomMinimumSize = new Vector2(220, 0)
      };
      row.AddChild(label);

      var cost = _logic.GetCost(upgrade);
      var btn = new Button { Text = upgrade.Level >= upgrade.MaxLevel ? "MAX" : $"Buy ({cost})" };
      btn.Disabled = upgrade.Level >= upgrade.MaxLevel || c.Inventory.Coins < cost;
      var capturedUpgrade = upgrade;
      btn.Pressed += () =>
      {
        var (nextInv, nextProg) = _logic.Purchase(c.Inventory, MetaProgressionState.Default, capturedUpgrade.Kind);
        c.SetInventory(nextInv);
        ApplyUpgrades(nextProg);
        Build();
        _ = SaveAsync();
      };

      row.AddChild(btn);
      list.AddChild(row);
    }
  }

  private void ApplyUpgrades(MetaProgressionState prog)
  {
    var c = BaseContainer.Instance;
    foreach (var (_, upgrade) in prog.Upgrades)
    {
      switch (upgrade.Kind)
      {
        case MetaUpgradeKind.MaxHealth:
          c.UpgradePlayerMaxHealth(upgrade.Level * upgrade.ValuePerLevel);
          break;
        case MetaUpgradeKind.Damage:
          break;
        case MetaUpgradeKind.Speed:
          break;
      }
    }
  }

  private async Task SaveAsync()
  {
    var c = BaseContainer.Instance;
    try
    {
      await c.Persistence.AutoSaveRunAsync(c.Score.CurrentScore, c.WaveController.CurrentWaveIndex + 1, c.Inventory.Coins);
    }
    catch (System.Exception ex)
    {
      GD.Print("Save failed ", ex.Message);
    }
  }
}
