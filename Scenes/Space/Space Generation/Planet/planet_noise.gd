@tool
class_name PlanetNoise extends Resource

@export var use_first_layer_as_mask: bool = false:
	set(value):
		use_first_layer_as_mask = value
		emit_changed()

@export var min_height: float = 10.0:
	set(value):
		min_height = value
		emit_changed()

@export var amplitude: float = 10.0:
	set(value):
		amplitude = value
		emit_changed()

@export var noise_map: NoiseTexture2D:
	set(value):
		noise_map = value
		if value.noise:
			noise_map.noise = value.noise
		emit_changed()
