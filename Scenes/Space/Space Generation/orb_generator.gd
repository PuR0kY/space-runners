class_name OrbGenerator
extends SpaceGenerator

@export var orb_scene: PackedScene
@export var orb_count: int = 30

func generate() -> void:
	for i in orb_count:
		if orb_scene:
			var orb = orb_scene.instantiate()
			orb.transform.origin = _create_vector_in_spawn_radius()
			orb.scale = Vector3(40, 40, 40)
			add_child(orb)
