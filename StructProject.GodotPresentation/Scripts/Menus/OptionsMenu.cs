using Godot;

namespace StructProject.GodotPresentation.Scripts.Menus;

public partial class OptionsMenu : Control
{
  [Export] private NodePath MasterPath { get; set; } = "VBox/MasterVolume";
  [Export] private NodePath SfxPath { get; set; } = "VBox/SfxVolume";
  [Export] private NodePath WindowModePath { get; set; } = "VBox/WindowMode";
  [Export] private NodePath BackButtonPath { get; set; } = "VBox/Back";
  [Export] public string MainMenuScenePath { get; set; } = "res://scenes/ui/main_menu.tscn";

  public override void _Ready()
  {
    var master = GetNode<HSlider>(MasterPath);
    var sfx = GetNode<HSlider>(SfxPath);
    var windowMode = GetNode<OptionButton>(WindowModePath);

    master.Value = AudioServer.GetBusVolumeDb(0);
    sfx.Value = AudioServer.GetBusVolumeDb(1);

    master.ValueChanged += v => AudioServer.SetBusVolumeDb(0, (float)v);
    sfx.ValueChanged += v => AudioServer.SetBusVolumeDb(1, (float)v);

    windowMode.AddItem("Windowed", 0);
    windowMode.AddItem("Borderless", 1);
    windowMode.AddItem("Fullscreen", 2);
    var win = GetWindow();
    var initial = win.Mode;
    windowMode.Selected = initial switch
    {
      Window.ModeEnum.Fullscreen => 2,
      Window.ModeEnum.Maximized => 1,
      _ => 0
    };

    windowMode.ItemSelected += idx =>
    {
      win.Mode = idx switch
      {
        1 => Window.ModeEnum.Maximized,
        2 => Window.ModeEnum.Fullscreen,
        _ => Window.ModeEnum.Windowed
      };
    };

    GetNode<Button>(BackButtonPath).Pressed += () =>
      GetTree().ChangeSceneToFile(MainMenuScenePath);
  }
}
