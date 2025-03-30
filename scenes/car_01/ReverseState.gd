extends "res://scenes/state_machine/State.gd"
class_name ReverseState

@export var car: Car01

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass  # No initialization needed here

func enter() -> void:
	print("Entered ReverseState")
	car.visual_display_interface_sprite.visible = true
	UISignalBus.emit_gear_changed("Reverse")
	car.alarm_sfx_player.playing = true
	car.sensor_warning_label.visible = car.disable_sensors

func physics_update(delta: float) -> void:
	# Emergency braking & alarm systems
	var emergency_braking: bool = false
	if not car.alarm_muted and not car.disable_sensors:
		var min_dist: float = INF  # GDScript uses INF instead of float.MaxValue
		for obj in car.collision_objects.get_children():
			var obj_body = obj as StaticBody3D
			var obj_mesh = obj.get_child(0) as MeshInstance3D
			
			var distance = car.calculate_object_distance(obj_mesh)
			var proximity = car.is_object_in_proximity(obj_body, obj_mesh)
			
			# Find nearest object
			if distance <= car.object_alarm_distance:  # && proximity
				min_dist = min(min_dist, distance)
			
			# Enable emergency braking?
			if distance <= car.auto_braking_distance and proximity:
				emergency_braking = true
		
		# Calculate alarm frequency
		if min_dist <= car.object_alarm_distance:
			car.alarm_sfx_player.stream_paused = false
			car.alarm_sfx_player.pitch_scale = lerp(0.8, 0.5, min_dist / car.object_alarm_distance)
		else:
			car.alarm_sfx_player.stream_paused = true
	else:
		car.alarm_sfx_player.stream_paused = true
	
	# Acceleration physics
	var is_accelerating = Input.is_action_pressed("accelerate")
	if is_accelerating:
		var speed = car.linear_velocity.length()
		if speed < 2.5 and speed > 0.01:
			car.engine_force = -clamp(car.engine_force_value * car.brake_strength / speed, 0.0, 100.0)
		else:
			car.engine_force = -car.engine_force_value * 0.5
		
		car.engine_force *= Input.get_action_strength("accelerate")
	else:
		car.engine_force = 0.0
	
	# Braking physics
	var is_braking = emergency_braking or Input.is_action_pressed("reverse")
	if is_braking:
		car.engine_force = 0.0
		car.brake = car.brake_strength * 2.5
	
	# UI update signals
	UISignalBus.emit_braking_pressed(is_braking)
	UISignalBus.emit_accelerating_pressed(is_accelerating)
	
	# State change
	if not car.force_reverse and Input.is_action_just_pressed("shift_gear"):
		emit_signal("transitioned", "ForwardState")
