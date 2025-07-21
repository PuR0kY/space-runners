extends Node3D

@export var orb_scene: PackedScene
@export var orb_count: int = 30
@export var radius: float = 10000.0

func _create_vector_in_spawn_radius() -> Vector3:
	return Vector3(
		randf_range(-radius, radius),
		randf_range(-radius, radius),
		randf_range(-radius, radius)
	)

func _ready() -> void:
	for i in orb_count:
		if orb_scene:
			var orb_to_spawn = orb_scene.instantiate()
			orb_to_spawn.transform.origin = _create_vector_in_spawn_radius()
			var scaleIndex = 40
			orb_to_spawn.scale = Vector3(scaleIndex, scaleIndex, scaleIndex)
			add_child(orb_to_spawn)
