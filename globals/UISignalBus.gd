extends Node

# Signal definitions
signal gear_changed(gear: String)
signal velocity_changed(velocity: float)
signal object_changed(obj: StaticBody3D, distance: float, proximity: bool)
signal accelerating_pressed(accelerating: bool)
signal braking_pressed(braking: bool)
signal camera_cycled(camera: String)
signal scenario_changed(scenario: int, collision_objects: Node3D)
signal alarm_muted(alarm_muted: bool)
signal collision_detected(collision_detected: bool)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass  # No need for explicit Instance assignment in GDScript autoload

# Signal emission functions
func emit_scenario_changed(scenario: int, collision_objects: Node3D) -> void:
	emit_signal("scenario_changed", scenario, collision_objects)

func emit_alarm_muted(alarm_muted: bool) -> void:
	emit_signal("alarm_muted", alarm_muted)

func emit_collision_detected(collision_detected: bool) -> void:
	emit_signal("collision_detected", collision_detected)

func emit_gear_changed(gear: String) -> void:
	emit_signal("gear_changed", gear)

func emit_velocity_changed(velocity: float) -> void:
	emit_signal("velocity_changed", velocity)

func emit_object_changed(obj: StaticBody3D, distance: float, proximity: bool) -> void:
	emit_signal("object_changed", obj, distance, proximity)

func emit_accelerating_pressed(accelerating: bool) -> void:
	emit_signal("accelerating_pressed", accelerating)

func emit_braking_pressed(braking: bool) -> void:
	emit_signal("braking_pressed", braking)

func emit_camera_cycled(camera: String) -> void:
	emit_signal("camera_cycled", camera)
