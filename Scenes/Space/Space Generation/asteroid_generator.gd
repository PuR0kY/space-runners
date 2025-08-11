class_name AsteroidGenerator
extends SpaceGenerator

@export var asteroid_count: int = 1200
@export var asteroid_1: PackedScene
@export var asteroid_2: PackedScene

func _create_random_rotation() -> Vector3:
	return Vector3(
		rng.randf_range(0, 360),
		rng.randf_range(0, 360),
		rng.randf_range(0, 360)
	)

func generate_asteroid(index: int) -> Node:
	var asteroid
	if (index == 1):
		asteroid = asteroid_1.instantiate()
	else:
		asteroid = asteroid_2.instantiate()

	asteroid.transform.origin = _create_vector_in_spawn_radius()
	asteroid.rotation = _create_random_rotation()
	var scale = rng.randf_range(1.0, 180.0)
	asteroid.scale = Vector3(scale, scale, scale)
	return asteroid

func generate():
	for i in asteroid_count:
		var index = randi_range(1, 2)
		var asteroid = generate_asteroid(index)
		add_child(asteroid)
