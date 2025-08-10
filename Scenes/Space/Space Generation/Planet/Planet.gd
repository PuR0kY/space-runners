@tool
extends Node3D
class_name Planet

@export var planet_data: PlanetData
@export var rotation_speed_deg: float = 1.0 # stupně za sekundu
@export var gravity_strength: float = 20.0  # m/s^2
@export var gravity_range: float = 1000.0       # maximální dosah gravitačního pole
@onready var area_3d: Area3D = $Area3D

func _ready() -> void:
	if planet_data:
		for n in planet_data.planet_noises:
			if n.noise_map and not n.noise_map.is_connected("changed", on_data_changed):
				n.noise_map.changed.connect(on_data_changed)
	on_data_changed()
	area_3d.monitorable = true
	area_3d.monitoring = true
	

func _process(delta: float) -> void:
	rotate_y(deg_to_rad(rotation_speed_deg) * delta)
	
func _physics_process(delta: float) -> void:
	for body in area_3d.get_overlapping_bodies():
		if body is Spaceship:
				print("spaceship entered", body.ship_id)
				var dir: Vector3 = global_transform.origin - body.global_transform.origin
				var dist: float = dir.length()

				var force: Vector3 = dir.normalized() * gravity_strength * (1.0 - dist / gravity_range)
				print("adding force of: ", force)
				(body as Spaceship).set_gravity_force(force * -1)


func on_data_changed():
	if not planet_data:
		return
	planet_data.min_height = 99999.0
	planet_data.max_height = 0.0
	for child in get_children():
		if child is PlanetMeshFace:
			child.regenerate_mesh(planet_data)
	print("regenerating...")


func _on_area_3d_body_exited(body: Node3D) -> void:
	if body is Spaceship:
			print("spaceship left", body.ship_id)
			(body as Spaceship).set_gravity_force(Vector3(0.0, 0.0, 0.0))
