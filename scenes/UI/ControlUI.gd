extends Control

@export var collision_objects: Node3D

@onready var gear_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer/GearLabel
@onready var velocity_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer/VelocityLabel
@onready var accelerating_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer2/AcceleratingLabel
@onready var braking_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer2/BrakingLabel
@onready var camera_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer3/CameraLabel
@onready var scenario_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer3/ScenarioLabel
@onready var scenario_description_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer2/ScrollContainer/ScenarioDescriptionLabel
@onready var muted_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer4/MutedLabel
@onready var collided_label: Label = $Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer4/CollidedLabel
@onready var object_panel_container: HBoxContainer = $Panel/MarginContainer/HBoxContainer/ObjectPanelContainer

var object_panels: Dictionary = {}
var scenario_descriptions: Dictionary = {
	1: "Activation Of System (General Use, no detection and detection):\nDriver shifts the vehicle into reverse. This activates the Pedestrian Backup Assistance System and displays to the driver the rear blindspot. The Pedestrian Backup Assistance System continually monitors the vehicle’s rear trajectory alerting the driver of potential obstacles. During the reverse, an obstacle crosses the rear of the vehicle and is detected by the system.  The system calculates the distance to the obstacle and determines if it needs to give visual and audible warnings to the driver based on the distance. After the driver has finished reversing the vehicle, they will shift the vehicle out of reverse and the system will deactivate.",
	2: "Vehicle Alerts Driver of Obstacle and Driver Fails to Stop (Automatic Braking):\nDriver shifts the vehicle into reverse. This activates the Pedestrian Backup Assistance System and displays to the driver the rear blindspot. An obstacle crosses begins to cross the rear of the vehicle and a visual and audible warning is given to the driver. The driver is unable to react in time and continues to reverse the vehicle.  Once the vehicle detects that the obstacle is within 1 meter of the rear bumper the system will automatically engage the brakes. The driver can disengage the brakes by holding the brakes and letting go. Once the driver is finished reversing and shifts out of the reverse gear, the system will deactivate.",
	3: "Vehicle Detects Non Existent Obstacle (Temporarily Mute Alarm):\nDriver is reversing the vehicle and the Pedestrian Backup Assistance System falsely detects there is an obstacle and gives visual and audible warnings to the driver. The driver checks the camera’s feed to see where the obstacle is and sees there is no obstacle. They push a button on the dashboard to mute the warnings and continue reversing. The system does not continue giving warning to the driver until it sees a new obstacle enter the rear blindspot.",
	4: "Vehicle Collided with Object (Immobilize Vehicle):\nDriver is reversing the vehicle. An obstacle is fast approaching our vehicle and the system displays warnings to the driver. The driver stops their car, but the obstacle continues on its path and collides with the vehicle. Upon the system detecting the obstacle with the collision detector, the system automatically engages the brakes immobilizing the vehicle.",
	5: "System Detects Sensors are Faulty:\nIf the system detects that less than 2/3 of the sensors are faulty, it notifies the driver that some sensors are malfunctioning but does not shut down. If the system detects that more than 2/3 of the sensors are faulty, it alerts the driver that the system is no longer functional and automatically deactivates.",
	6: "View Rear Blindspot:\nWhen the system is activated, the system detects objects in the backup camera’s vicinity and creates bounding boxes around the obstacles to identify each object for the driver."
}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	# Connect signals from UISignalBus
	UISignalBus.connect("gear_changed", _on_gear_changed)
	UISignalBus.connect("velocity_changed", _on_velocity_changed)
	UISignalBus.connect("object_changed", _on_object_changed)
	UISignalBus.connect("accelerating_pressed", _on_accelerating_pressed)
	UISignalBus.connect("braking_pressed", _on_braking_pressed)
	UISignalBus.connect("camera_cycled", _on_camera_cycled)
	UISignalBus.connect("scenario_changed", _on_scenario_changed)
	UISignalBus.connect("alarm_muted", _on_alarm_muted)
	UISignalBus.connect("collision_detected", _on_collision_detected)

func _on_scenario_changed(scenario: int, collision_objects_node: Node3D) -> void:
	# Update UI labels
	scenario_label.text = "Scenario: (%d)" % scenario
	scenario_description_label.text = scenario_descriptions[scenario]
	
	# Clear existing object panels
	for obj_panel in object_panel_container.get_children():
		obj_panel.queue_free()
	
	# Init object panels
	collision_objects = collision_objects_node
	object_panels.clear()  # Reset dictionary
	for obj in collision_objects.get_children():
		var obj_body = obj as StaticBody3D
		if obj_body:
			var panel = load("res://scenes/UI/ObjectPanel.tscn").instantiate() as ObjectPanel
			object_panels[obj_body] = panel
			object_panel_container.add_child(panel)
			panel.name_label.text = obj_body.name

func _on_collision_detected(collision_detected: bool) -> void:
	collided_label.text = "Collision: %s" % collision_detected

func _on_alarm_muted(muted: bool) -> void:
	muted_label.text = "Muted: %s" % muted

func _on_gear_changed(gear: String) -> void:
	gear_label.text = "Gear: %s" % gear

func _on_velocity_changed(velocity: float) -> void:
	velocity_label.text = "Velocity: %.3f" % velocity

func _on_object_changed(obj: StaticBody3D, distance: float, proximity: bool) -> void:
	if obj in object_panels:
		object_panels[obj].distance_label.text = "Distance: %.3f" % distance
		object_panels[obj].proximity_label.text = "Proximity: %s" % proximity

func _on_accelerating_pressed(accelerating: bool) -> void:
	accelerating_label.text = "Accelerating: %s" % accelerating

func _on_braking_pressed(braking: bool) -> void:
	braking_label.text = "Braking: %s" % braking

func _on_camera_cycled(camera: String) -> void:
	camera_label.text = "Camera: %s" % camera
