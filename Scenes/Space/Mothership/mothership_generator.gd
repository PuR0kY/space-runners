extends Node3D

@export var mothership_scene: PackedScene
@export var radius: float = 10000.0

func _create_vector_in_spawn_radius() -> Vector3:
	return Vector3(
		randf_range(-radius, radius),
		randf_range(-radius, radius),
		randf_range(-radius, radius)
	)
	
func _spawn_mothership() -> void:
	if mothership_scene:
			var mothership = mothership_scene.instantiate()
			mothership.transform.origin = _create_vector_in_spawn_radius()
			mothership.scale = Vector3(120.0, 120.0, 120.0)
			add_child(mothership)

func _ready() -> void:
	
	# TODO: Team Separation
	_spawn_mothership() # Team 1
	_spawn_mothership() # Team 2
