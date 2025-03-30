extends StaticBody3D

const MOVE_SPEED = 5.0  # Base movement speed (m/s)
const TURN_SPEED = 1.5  # Rotation speed (radians/s)
const ACCELERATION = 2.0  # How quickly it reaches target speed

@export var max_speed := 5.0
@export var target: Node3D  # The target node to move towards

var current_speed := 0.0
var previous_speed := 0.0
var collided := false

@onready var desired_engine_pitch: float = $EngineSound.pitch_scale
@onready var collision_detector: Area3D = $CollisionDetector

func _ready() -> void:
	collision_detector.body_entered.connect(_on_collision_detected)

func _on_collision_detected(body):
	print("Truck collided.")
	collided = true

func _physics_process(delta: float) -> void:
	if target and not collided:
		# Calculate direction to target
		var target_direction = (global_position - target.global_position).normalized()
		
		# Calculate rotation
		# var target_rotation = atan2(-target_direction.x, -target_direction.z)
		# var current_rotation = rotation.y
		# var new_rotation = lerp_angle(current_rotation, target_rotation, TURN_SPEED * delta)
		# rotation.y = new_rotation
		
		# Accelerate towards target
		current_speed = lerp(current_speed, max_speed, ACCELERATION * delta)
	else:
		# Decelerate when no target or collided
		current_speed = lerp(current_speed, 0.0, ACCELERATION * delta)

	# Engine sound simulation
	# desired_engine_pitch = 0.05 + current_speed / (max_speed * 0.5)
	# $EngineSound.pitch_scale = lerpf($EngineSound.pitch_scale, desired_engine_pitch, 0.2)

	# Collision detection (using speed difference)
	if absf(current_speed - previous_speed) > 1.0 and current_speed < previous_speed:
		$ImpactSound.play()
		collided = true

	# Move the body
	if not collided:
		var forward = global_transform.basis.z.normalized()
		var velocity = forward * current_speed
		global_position += velocity * delta

	previous_speed = current_speed
