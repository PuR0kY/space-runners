extends Node3D

@onready var picking_scene: Node2D = $PickingScene
var space = preload("res://Scenes/Space/Space.tscn")
var space_instance

func _ready():
	pass

func _on_exit_pressed() -> void:
	get_tree().quit()

func _on_hangar_pressed() -> void:
	picking_scene.visible = !picking_scene.visible

func spawn_space() -> void:
	space_instance = space.instantiate()
	$"..".add_child(space_instance)
	(space_instance as Space).setup_generators()

func _on_play_pressed() -> void:
	spawn_space()
	GDSync.start_multiplayer()
	queue_free()
