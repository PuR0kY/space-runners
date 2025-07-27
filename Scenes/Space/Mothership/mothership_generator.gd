class_name MothershipGenerator
extends SpaceGenerator

@export var mothership_scene: PackedScene

func _spawn_mothership():
	if mothership_scene:
		var mothership = mothership_scene.instantiate()
		mothership.transform.origin = _create_vector_in_spawn_radius()
		mothership.scale = Vector3(120, 120, 120)
		add_child(mothership)

func generate():
	_spawn_mothership() # Team 1
	_spawn_mothership() # Team 2
