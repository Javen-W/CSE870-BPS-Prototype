extends Node3D

var current_scenario_index: int
var current_scenario: Node3D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	# Init UI scenario
	current_scenario_index = 1
	current_scenario = $Scenario1 as Node3D  # Assumes Scenario1 is a child node
	var collision_objects = current_scenario.get_child(2) as Node3D
	UISignalBus.emit_scenario_changed(current_scenario_index, collision_objects)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	handle_scenario_change_input()

func change_scenario_scene(scenario_index: int, scenario_path: String) -> void:
	current_scenario_index = scenario_index
	if current_scenario != null:
		current_scenario.queue_free()
	current_scenario = load(scenario_path).instantiate() as Node3D
	add_child(current_scenario)
	var collision_objects = current_scenario.get_child(2) as Node3D
	UISignalBus.emit_scenario_changed(current_scenario_index, collision_objects)

func handle_scenario_change_input() -> void:
	if Input.is_action_just_pressed("scenario1"):
		change_scenario_scene(1, "res://scenes/scenarios/Scenario1.tscn")
	#elif Input.is_action_just_pressed("scenario2"):
		#change_scenario_scene(2, "res://scenes/scenarios/Scenario2.tscn")
	elif Input.is_action_just_pressed("scenario3"):
		change_scenario_scene(2, "res://scenes/scenarios/Scenario3.tscn")
	elif Input.is_action_just_pressed("scenario4"):
		change_scenario_scene(3, "res://scenes/scenarios/Scenario4.tscn")
	elif Input.is_action_just_pressed("scenario5"):
		change_scenario_scene(4, "res://scenes/scenarios/Scenario5.tscn")
	#elif Input.is_action_just_pressed("scenario6"):
		#change_scenario_scene(6, "res://scenes/scenarios/Scenario6.tscn")
