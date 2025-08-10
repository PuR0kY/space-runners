@tool
extends Resource
class_name PlanetData

@export var radius: float = 10.0:
	set(value):
		radius = value
		emit_changed()
		
@export var resolution: float = 10.0:
	set(value):
		resolution = value
		emit_changed()
		
@export var planet_noises: Array[PlanetNoise]:
	set(noises):
		planet_noises = noises
		emit_changed()

func point_on_planet(point_on_sphere: Vector3) -> Vector3:
	var elevation = 0.0
	for n in planet_noises:
		var level_elevation = n.noise_map.noise.get_noise_3dv(point_on_sphere * 100.0)
		level_elevation = level_elevation + 1.0 / 2.0 * n.amplitude
		level_elevation = max(0.0, level_elevation - n.min_height)
		elevation += level_elevation
	return point_on_sphere * radius * (elevation + 1.0)
