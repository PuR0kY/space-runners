extends Node3D

@onready var ship: Node3D = $Ship

func _ready() -> void:
	pass
	
func _on_exit_pressed() -> void:
	get_tree().quit()
