extends "res://scenes/state_machine/State.gd"
class_name ForwardState

@export var car: Car01  # Placeholder; replace with actual Car01 type (e.g., VehicleBody3D)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass  # No initialization needed here

func enter() -> void:
	print("Entered ForwardState")
	UISignalBus.emit_gear_changed("Forward")
	if not car.always_display_backup_camera:
		car.visual_display_interface_sprite.visible = false
	car.alarm_sfx_player.playing = false

func physics_update(delta: float) -> void:
	# Acceleration physics
	var is_accelerating = Input.is_action_pressed("accelerate")
	if is_accelerating:
		var speed = car.linear_velocity.length()
		if speed < 5.0 and speed > 0.01:
			car.engine_force = clamp(car.engine_force_value * 5.0 / speed, 0.0, 100.0)
		else:
			car.engine_force = car.engine_force_value
		
		car.engine_force *= Input.get_action_strength("accelerate")
	else:
		car.engine_force = 0.0
		car.brake = car.brake_strength * 0.5
	
	# Braking physics
	var is_braking = Input.is_action_pressed("reverse")
	if is_braking:
		car.engine_force = 0.0
		car.brake = car.brake_strength
	
	# UI update signals
	UISignalBus.emit_braking_pressed(is_braking)
	UISignalBus.emit_accelerating_pressed(is_accelerating)
	
	# State change
	if Input.is_action_just_pressed("shift_gear"):
		emit_signal("transitioned", "ReverseState")
