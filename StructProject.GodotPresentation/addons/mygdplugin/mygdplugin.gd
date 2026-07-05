@tool
extends EditorPlugin

# Control Dock
var _dock: Control

func _enable_plugin() -> void:
	# Add autoloads here.
	pass


func _disable_plugin() -> void:
	# Remove autoloads here.
	pass


func _enter_tree() -> void:
	# Initialization of the plugin goes here.
	_dock = VBoxContainer.new()
	var label = Label.new()
	label.text = "MyGDPlugin is active!"
	_dock.add_child(label)
	add_control_to_dock(DockSlot.DOCK_SLOT_RIGHT_UL, _dock)


func _exit_tree() -> void:
	# Clean-up of the plugin goes here.
	remove_control_from_docks(_dock)
	_dock.queue_free()
