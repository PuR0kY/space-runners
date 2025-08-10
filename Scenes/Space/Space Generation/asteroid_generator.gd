class_name AsteroidGenerator
extends SpaceGenerator

@export var mesh_library: MeshLibrary
@export var asteroid_scene: PackedScene
@export var asteroid_count: int = 1200

func _create_random_rotation() -> Vector3:
	return Vector3(
		rng.randf_range(0, 360),
		rng.randf_range(0, 360),
		rng.randf_range(0, 360)
	)

func generate_asteroid(mesh: Mesh) -> Node:
	var asteroid = asteroid_scene.instantiate()
	asteroid.mesh_instance = mesh
	asteroid.transform.origin = _create_vector_in_spawn_radius()
	asteroid.rotation = _create_random_rotation()
	var scale = rng.randf_range(0.01, 800.0)
	asteroid.scale = Vector3(scale, scale, scale)
	return asteroid

func generate():
	if not mesh_library:
		push_error("MeshLibrary is not assigned.")
		return

	var mesh_ids = mesh_library.get_item_list()
	if mesh_ids.is_empty():
		push_error("MeshLibrary is empty.")
		return

	for i in asteroid_count:
		var mesh_id = mesh_ids[rng.randi() % mesh_ids.size()]
		var mesh = mesh_library.get_item_mesh(mesh_id)
		if mesh:
			var asteroid = generate_asteroid(mesh)
			add_child(asteroid)
