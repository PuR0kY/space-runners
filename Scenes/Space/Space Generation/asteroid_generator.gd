extends Node3D

@export var mesh_library: MeshLibrary
@export var asteroid_scene: PackedScene
@export var asteroid_count: int = 1200
@export var spawn_radius: float = 10000.0

func _create_vector_in_spawn_radius() -> Vector3:
	return Vector3(
		randf_range(-spawn_radius, spawn_radius),
		randf_range(-spawn_radius, spawn_radius),
		randf_range(-spawn_radius, spawn_radius)
	)
	
func _create_random_rotation() -> Vector3:
	return Vector3(
		randf_range(0, 360),
		randf_range(0, 360),
		randf_range(0, 360)
	)

func generate_asteroid(mesh: Mesh) -> Node:
	var asteroid = asteroid_scene.instantiate()
	asteroid.mesh_instance = mesh
	asteroid.transform.origin = _create_vector_in_spawn_radius()
	asteroid.rotation = _create_random_rotation()
	
	var scaleIndex = randf_range(.01, 40.0)
	asteroid.scale = Vector3(scaleIndex, scaleIndex, scaleIndex)
	
	return asteroid

func _ready():
	if not mesh_library:
		push_error("MeshLibrary is not assigned.")
		return

	var mesh_ids = mesh_library.get_item_list()
	if mesh_ids.is_empty():
		push_error("MeshLibrary is empty.")
		return

	for i in asteroid_count:
		var mesh_id = mesh_ids[randi() % mesh_ids.size()]
		var mesh = mesh_library.get_item_mesh(mesh_id)

		if mesh:
			var asteroid = generate_asteroid(mesh)
			add_child(asteroid)
