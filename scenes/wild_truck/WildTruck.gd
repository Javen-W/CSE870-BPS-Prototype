extends VehicleBody3D

const STEER_SPEED = 1.5
const STEER_LIMIT = 0.4
const BRAKE_STRENGTH = 2.0

@export var engine_force_value := 40.0
@export var target: Node3D  # The target node to move towards

var previous_speed := linear_velocity.length()
var _steer_target := 0.0
var collided := false

@onready var desired_engine_pitch: float = $EngineSound.pitch_scale

func _physics_process(delta: float) -> void:
	if target and not collided:
		# Calculate direction to target
		var target_direction = (target.global_position - global_position).normalized()
		
		# Convert direction to local space for steering
		var local_direction = global_transform.basis.inverse() * target_direction
		
		# Calculate steering based on the x component of local direction
		_steer_target = clamp(-local_direction.x, -STEER_LIMIT, STEER_LIMIT)
	else:
		_steer_target = 0.0

	# Engine sound simulation
	desired_engine_pitch = 0.05 + linear_velocity.length() / (engine_force_value * 0.5)
	$EngineSound.pitch_scale = lerpf($EngineSound.pitch_scale, desired_engine_pitch, 0.2)

	# Collision detection
	if absf(linear_velocity.length() - previous_speed) > 1.0:
		$ImpactSound.play()
		collided = true

	# Engine force control
	if not collided and target:
		# Increase engine force at low speeds
		var speed := linear_velocity.length()
		if speed < 5.0 and not is_zero_approx(speed):
			engine_force = clampf(engine_force_value * 5.0 / speed, 0.0, 100.0)
		else:
			engine_force = engine_force_value
	else:
		engine_force = 0.0

	steering = move_toward(steering, _steer_target, STEER_SPEED * delta)
	previous_speed = linear_velocity.length()
