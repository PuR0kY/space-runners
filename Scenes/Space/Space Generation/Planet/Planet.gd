@tool
extends Node3D
class_name Planet

@export var planet_data: PlanetData

func _ready() -> void:
	if planet_data:
		planet_data.changed.connect(on_data_changed)
		for n in planet_data.planet_noises:
			if n.noise_map and not n.noise_map.is_connected("changed", on_data_changed):
				n.noise_map.changed.connect(on_data_changed)
	on_data_changed()

func on_data_changed():
	if not planet_data:
		return

	for child in get_children():
		var face := child as PlanetMeshFace
		if face:
			face.regenerate_mesh(planet_data)
	print("regenerating...")
