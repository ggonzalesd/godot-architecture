namespace StructProject.GodotPresentation.addons.MyPlugin;

#if TOOLS
using Godot;

[Tool]
public partial class MyPlugin : EditorPlugin
{
  private Control? _dock;

  public override void _EnterTree()
  {
    _dock = new VBoxContainer();

    _dock.AddChild(
      new Label
      {
        Text = "MyPlugin is active!",
      }
    );

    _dock.Name = "MyPluginDock";

    // AddControlToDock(DockSlot.RightUl, _dock);
  }

  public override void _ExitTree()
  {
    // RemoveControlFromDocks(_dock);
    _dock?.QueueFree();
  }
}
#endif
