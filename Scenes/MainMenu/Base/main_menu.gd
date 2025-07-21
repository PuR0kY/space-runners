extends Node3D

@onready var ship: Node3D = $Ship
@onready var picking_scene: Node2D = $PickingScene

func _ready() -> void:
	pass
	
func _on_exit_pressed() -> void:
	get_tree().quit()


func _on_hangar_pressed() -> void:
	picking_scene.visible = !picking_scene.visible
