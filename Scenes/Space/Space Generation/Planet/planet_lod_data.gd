@tool
extends Resource
class_name PlanetLODData

@export var mesh_id: int
@export var lod_out: Array = []
@export var lod_far: Array = []
@export var lod_mid: Array = []
@export var lod_close: Array = []
@export var material: Material

func set_lod(resolution: int, arrays: Array) -> void:
	match resolution:
		50: lod_out = arrays
		150: lod_far = arrays
		350: lod_mid = arrays
		650: lod_close = arrays

func get_lod(resolution: int) -> Array:
	match resolution:
		50: return lod_out
		150: return lod_far
		350: return lod_mid
		650: return lod_close
		_: return []
