extends VehicleBody3D
class_name Car01

@export var steer_speed: float = 1.5
@export var steer_limit: float = 0.4
@export var brake_strength: float = 2.0
@export var engine_force_value: float = 40.0
@export var auto_braking_distance: float = 3.0
@export var object_alarm_distance: float = 8.0
@export var always_display_backup_camera: bool = false
@export var alarm_muted: bool = false
@export var collision_objects: Node3D
@export var disable_steering: bool = false
@export var force_reverse: bool = false
@export var disable_sensors: bool = false

@onready var state_machine: StateMachine = $StateMachine
@onready var sub_viewport_rear: SubViewport = $Cameras/SubViewportRear
@onready var visual_display_interface_sprite: Sprite3D = $VisualDisplayInterface/Sprite3D
@onready var camera_inside: Camera3D = $Cameras/Camera3DInside
@onready var camera_side: Camera3D = $Cameras/Camera3DSide
@onready var camera_top: Camera3D = $Cameras/Camera3DTop
@onready var camera_rear: Camera3D = $Cameras/SubViewportRear/Camera3DRear
@onready var proximity_sensor_array: ProximitySensorArray = $ProximitySensorArray
@onready var collision_detection_sensor: CollisionDetectionSensor = $CollisionDetectionSensor
@onready var alarm_sfx_player: AudioStreamPlayer3D = $AlarmSFXPlayer
@onready var crash_sfx_player: AudioStreamPlayer3D = $CrashSFXPlayer

var steer_target: float
var previous_speed: float
var _target_rects: Dictionary = {}
var _camera_index: int = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	# Init states
	previous_speed = linear_velocity.length()
	_target_rects = {}
	handle_camera_cycled(false)
	UISignalBus.emit_alarm_muted(alarm_muted)
	UISignalBus.emit_collision_detected(false)
	
	if not force_reverse:
		state_machine.transition_state("ForwardState")
	else:
		state_machine.transition_state("ReverseState")
	
	# Init signals
	collision_detection_sensor.collision_detected.connect(_on_collision_detected)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta: float) -> void:
	# Common I/O events
	if not disable_steering:
		steer_target = Input.get_axis("turn_right", "turn_left")
		steer_target *= steer_limit
	if Input.is_action_just_pressed("cycle_camera"):
		handle_camera_cycled(true)
	if Input.is_action_just_pressed("mute_alarm"):
		alarm_muted = not alarm_muted
		UISignalBus.emit_alarm_muted(alarm_muted)
	
	# Process state machine
	state_machine.physics_update(delta)
	
	# Common physics operations
	steering = move_toward(steering, steer_target, steer_speed * delta)
	previous_speed = linear_velocity.length()
	UISignalBus.emit_velocity_changed(previous_speed)
	
	# Object highlighting & UI updates
	for obj in collision_objects.get_children():
		# Update reference rectangles
		var obj_body = obj as StaticBody3D
		var obj_mesh = obj.get_child(0) as MeshInstance3D
		if not disable_sensors:
			update_target_rect(camera_rear, obj_mesh)
		var distance = calculate_object_distance(obj_mesh)
		var proximity = is_object_in_proximity(obj_body, obj_mesh)
		UISignalBus.emit_object_changed(obj_body, distance, proximity)

func calculate_object_distance(obj_mesh: MeshInstance3D) -> float:
	if disable_sensors:
		return -1.0
	return camera_rear.global_position.distance_to(obj_mesh.global_position)

func is_object_in_proximity(obj_body: StaticBody3D, obj_mesh: MeshInstance3D) -> bool:
	if disable_sensors:
		return false
	var in_sensor = proximity_sensor_array.detected_objects.has(obj_body)
	var in_fov = is_object_in_camera_fov(camera_rear, obj_mesh)
	return in_sensor or in_fov

func _on_collision_detected() -> void:
	print("OnCollisionDetected()")
	state_machine.transition_state("CollisionState")

func handle_camera_cycled(increment: bool) -> void:
	if increment:
		_camera_index = (_camera_index + 1) % 3
	camera_inside.current = _camera_index == 0
	camera_side.current = _camera_index == 1
	camera_top.current = _camera_index == 2
	
	if camera_inside.current:
		UISignalBus.emit_camera_cycled("Inside")
	elif camera_side.current:
		UISignalBus.emit_camera_cycled("Side")
	else:
		UISignalBus.emit_camera_cycled("Top")

func is_object_in_camera_fov(camera: Camera3D, obj_mesh: MeshInstance3D) -> bool:
	var camera_transform = camera.global_transform
	var direction = camera_transform.origin.direction_to(obj_mesh.global_position)
	
	var camera_forward = -camera_transform.basis.z.normalized()
	var forward_alignment = camera_forward.dot(direction)
	
	var fov_angle = acos(forward_alignment)
	var fov_radians = deg_to_rad(camera.fov) / 2.0
	
	return fov_angle <= fov_radians and forward_alignment > 0

func update_target_rect(camera: Camera3D, obj_mesh: MeshInstance3D) -> void:
	var distance = calculate_object_distance(obj_mesh)
	
	# If target isn't in FOV, remove its rect if it exists and return
	if not is_object_in_camera_fov(camera, obj_mesh):
		if obj_mesh in _target_rects:
			_target_rects[obj_mesh].queue_free()
			_target_rects.erase(obj_mesh)
		return
	
	# Create or get existing ReferenceRect
	var rect: ReferenceRect
	if not obj_mesh in _target_rects:
		rect = ReferenceRect.new()
		rect.editor_only = false
		rect.border_color = Color.GREEN
		rect.border_width = 4.0
		sub_viewport_rear.add_child(rect)
		_target_rects[obj_mesh] = rect
	else:
		rect = _target_rects[obj_mesh]
	
	# Use AABB for better bounds
	var aabb = obj_mesh.get_aabb()
	var corners = get_aabb_corners(aabb, obj_mesh.global_transform)
	var min_pos = Vector2(INF, INF)
	var max_pos = Vector2(-INF, -INF)
	
	for corner in corners:
		var corner_2d = camera.unproject_position(corner)
		min_pos.x = min(min_pos.x, corner_2d.x)
		min_pos.y = min(min_pos.y, corner_2d.y)
		max_pos.x = max(max_pos.x, corner_2d.x)
		max_pos.y = max(max_pos.y, corner_2d.y)
	
	# Calculate original size and position
	var original_size = max_pos - min_pos
	var original_center = min_pos + (original_size / 2.0)
	
	var shrunk_size = original_size * 0.75
	rect.position = original_center - (shrunk_size / 2.0)
	rect.size = shrunk_size
	
	# Interpolate color: Green (distance >= 10) to Red (distance = 0)
	var t = clamp(distance / 10.0, 0.0, 1.0)
	var green = Color.GREEN
	var red = Color.RED
	rect.border_color = green.lerp(red, 1.0 - t + 0.2)

func get_aabb_corners(aabb: AABB, transform: Transform3D) -> Array[Vector3]:
	var corners: Array[Vector3] = []
	var min_pos = aabb.position
	var max_pos = aabb.end
	
	corners.append(transform * Vector3(min_pos.x, min_pos.y, min_pos.z))
	corners.append(transform * Vector3(max_pos.x, min_pos.y, min_pos.z))
	corners.append(transform * Vector3(min_pos.x, max_pos.y, min_pos.z))
	corners.append(transform * Vector3(max_pos.x, max_pos.y, min_pos.z))
	corners.append(transform * Vector3(min_pos.x, min_pos.y, max_pos.z))
	corners.append(transform * Vector3(max_pos.x, min_pos.y, max_pos.z))
	corners.append(transform * Vector3(min_pos.x, max_pos.y, max_pos.z))
	corners.append(transform * Vector3(max_pos.x, max_pos.y, max_pos.z))
	
	return corners
