extends "res://scenes/state_machine/State.gd"
class_name CollisionState

@export var car: Car01  # Placeholder; replace with actual Car01 type (e.g., VehicleBody3D)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass  # No initialization needed here

func enter() -> void:
	print("Entered CollisionState")
	UISignalBus.emit_collision_detected(true)
	car.crash_sfx_player.playing = true
	car.alarm_sfx_player.playing = true
	car.alarm_sfx_player.stream_paused = false
	car.alarm_sfx_player.pitch_scale = 0.85
	
	car.sensor_warning_label.visible = false
	car.collision_warning_label.visible = true

func physics_update(delta: float) -> void:
	# Braking physics
	car.engine_force = 0.0
	car.brake = car.brake_strength * 1.5
	
	# UI update signals
	UISignalBus.emit_braking_pressed(true)
	UISignalBus.emit_accelerating_pressed(false)
