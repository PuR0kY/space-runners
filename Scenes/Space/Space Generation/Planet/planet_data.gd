@tool
extends Resource
class_name PlanetData

var min_height := 99999.0
var max_height := 0.0

@export var planet_color: GradientTexture1D:
	set(color):
		planet_color = color
		emit_changed()

@export var radius: float = 10.0:
	set(rad):
		radius = rad
		emit_changed()
		
@export var resolution: float = 10.0:
	set(res):
		resolution = res
		emit_changed()
		
@export var planet_noises: Array[PlanetNoise]:
	set(noises):
		planet_noises = noises
		emit_changed()

func point_on_planet(point_on_sphere: Vector3) -> Vector3:
	var elevation = 0.0
	var base_elevation
	if planet_noises.size() > 0:
		base_elevation = planet_noises[0].noise_map.noise.get_noise_3dv(point_on_sphere * 100)
		base_elevation = (base_elevation + 1) / 2.0 * planet_noises[0].amplitude
		base_elevation = max(0, base_elevation - planet_noises[0].min_height)
	for n in planet_noises:
		var mask := 1.0
		if n.use_first_layer_as_mask:
			mask = base_elevation
		var level_elevation = n.noise_map.noise.get_noise_3dv(point_on_sphere * 100.0)
		level_elevation = level_elevation + 1.0 / 2.0 * n.amplitude
		level_elevation = max(0.0, level_elevation - n.min_height) * mask
		elevation += level_elevation
	return point_on_sphere * radius * (elevation + 1.0)
