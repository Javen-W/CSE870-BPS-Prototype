extends Area3D

var detected_objects: Array[StaticBody3D] = []

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    print("ProximitySensorArray Ready()")
    detected_objects = []
    
    # Init signal connections
    body_entered.connect(_on_body_entered)
    body_exited.connect(_on_body_exited)

func _on_body_entered(body: Node) -> void:
    var static_body = body as StaticBody3D
    if static_body:
        detected_objects.append(static_body)
        print("ProximitySensorArray: detected_objs=", detected_objects.size())

func _on_body_exited(body: Node) -> void:
    var static_body = body as StaticBody3D
    if static_body:
        detected_objects.erase(static_body)
        print("ProximitySensorArray: detected_objs=", detected_objects.size())