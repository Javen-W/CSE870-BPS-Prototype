extends Control

@onready var name_label: Label = $Panel/MarginContainer/VBoxContainer/NameLabel
@onready var distance_label: Label = $Panel/MarginContainer/VBoxContainer/DistanceLabel
@onready var proximity_label: Label = $Panel/MarginContainer/VBoxContainer/ProximityLabel

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    pass  # Node references are already initialized with @onready