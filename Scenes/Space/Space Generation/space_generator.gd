class_name SpaceGenerator
extends Node3D

@export var radius: float = 10000.0
@export var seed: int = 123456

var rng: RandomNumberGenerator = RandomNumberGenerator.new()

func set_seed(new_seed: int) -> void:
	seed = new_seed
	rng.seed = seed

func _create_vector_in_spawn_radius() -> Vector3:
	return Vector3(
		rng.randf_range(-radius, radius),
		rng.randf_range(-radius, radius),
		rng.randf_range(-radius, radius)
	)

func _ready():
	# Nech prázdné nebo volitelně zavolej generate() zde
	pass

func generate() -> void:
	# Toto by se mělo přepsat v dědicích
	pass
