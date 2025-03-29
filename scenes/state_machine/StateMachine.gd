extends Node

@export var current_state: Node  # Assuming State is a Node-based class

var states: Dictionary = {}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    # Init states dictionary
    states = {}
    
    # Connect child state signals
    for child in get_children():
        if child is State:  # Replace 'State' with your actual State class name if different
            states[child.name] = child
            child.connect("transitioned", _on_child_transitioned.bind(child.name))
        else:
            push_warning("State machine contains child which is not 'State'")
    
    # Enter initial state
    if current_state != null:
        current_state.enter({})

# Analogous to _Process()
func update(delta: float) -> void:
    if current_state != null:
        current_state.update(delta)

# Analogous to _PhysicsProcess()
func physics_update(delta: float) -> void:
    if current_state != null:
        current_state.physics_update(delta)

func transition_state(new_state_name: String, args: Dictionary = {}) -> void:
    _on_child_transitioned(new_state_name, args)

func _on_child_transitioned(new_state_name: String, args: Dictionary = {}) -> void:
    var new_state = states.get(new_state_name)
    if new_state != null and new_state != current_state:
        print("StateMachine: OnChildTransitioned!")
        if current_state != null:
            current_state.exit()
        new_state.enter(args)
        current_state = new_state
    #else:
        #push_warning("Called transition on a state that does not exist")