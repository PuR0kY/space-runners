class_name Space
extends Node3D

@export var seed: int = 123456

@onready var asteroid_generator: AsteroidGenerator = $Space/PCG/Asteroids/AsteroidGenerator
@onready var mothership_generator: MothershipGenerator = $"Space/PCG/Motherships/Mothership Generator"
@onready var orb_generator: OrbGenerator = $"Space/PCG/Orbs/Orb Generator"

func _ready() -> void:
	pass

func setup_generators() -> void:
	asteroid_generator.set_seed(seed)
	asteroid_generator.generate()

	mothership_generator.set_seed(seed)
	mothership_generator.generate()

	orb_generator.set_seed(seed)
	orb_generator.generate()
