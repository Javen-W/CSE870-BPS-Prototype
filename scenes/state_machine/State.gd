extends Node

# Signal for transitioning to a new state
signal transitioned(new_state_name: String, args: Dictionary)

# Virtual methods
func enter(args: Dictionary) -> void:
    pass

func exit() -> void:
    pass

func update(_delta: float) -> void:
    pass

func physics_update(_delta: float) -> void:
    pass