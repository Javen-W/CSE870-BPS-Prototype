extends Node
class_name State

# Signal for transitioning to a new state
signal transitioned(new_state_name: String)

# Virtual methods
func enter() -> void:
	pass

func exit() -> void:
	pass

func update(_delta: float) -> void:
	pass

func physics_update(_delta: float) -> void:
	pass
