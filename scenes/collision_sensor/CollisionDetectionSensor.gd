extends Area3D
class_name CollisionDetectionSensor

# Signal for collision detection
signal collision_detected

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	# Init signal connections
	body_entered.connect(_on_body_entered)

func _on_body_entered(body: Node) -> void:
	print("CollisionDetectionSensor: OnBodyEntered ", body.name)
	emit_signal("collision_detected")
