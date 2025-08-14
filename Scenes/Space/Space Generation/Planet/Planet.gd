@tool
extends Node3D
class_name Planet

@export var rotation_speed_deg: float = 1.0 # stupně za sekundu
@export var gravity_strength: float = 20.0  # m/s^2
@export var gravity_range: float = 1000.0       # maximální dosah gravitačního pole
@onready var area_3d: Area3D = $Area3D

@export var min_height := 99999.0
@export var max_height := 0.0

@export var radius: float = 10.0:
	set(rad):
		radius = rad
		
@export var resolution: float = 50.0:
	set(res):
		resolution = res
		
@export var planet_noises: Array[PlanetNoise]:
	set(noises):
		planet_noises = noises

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

@onready var generate_new_meshes := false

var planet_id: int = 1

var current_resolution := 50

const CLOSE_RESOLUTION = 650
const MID_RESOLUTION = 350
const FAR_RESOLUTION = 150
const OUT_RESOLUTION = 50

var face_mesh_resolution := {
	0: PlanetLODData.new(),
	1: PlanetLODData.new(),
	2: PlanetLODData.new(),
	3: PlanetLODData.new(),
	4: PlanetLODData.new(),
	5: PlanetLODData.new()
}
	
var faces: Array[PlanetMeshFace] = []
var threads: Array[Thread] = []
	
func _ready() -> void:
	for child in get_children():
		if child is PlanetMeshFace:
			faces.append(child)

	# GENERATING NEW PLANET MESHES
	if generate_new_meshes:
		generate_lod_async(OUT_RESOLUTION)
		generate_lod_async(FAR_RESOLUTION)
		generate_lod_async(MID_RESOLUTION)
		generate_lod_async(CLOSE_RESOLUTION)
		save_faces_as_resources()
	
	# LOADING EXISTING MESHES
	load_faces_from_resources()
	apply_mesh(OUT_RESOLUTION)
		
func generate_lod_async(resolution: int) -> void:
	for child in faces:
		if child is PlanetMeshFace:
			print("creating mesh for face: ", child.mesh_id)
			print("with resolution: ", resolution)
			var arrays := child.regenerate_mesh(resolution)
			(face_mesh_resolution[child.mesh_id] as PlanetLODData).set_lod(resolution, arrays)

func apply_mesh(resolution: int) -> void:
	for child in faces:
		if child is PlanetMeshFace:
			child.update_mesh(face_mesh_resolution[child.mesh_id].get_lod(resolution))
	
func _process(delta: float) -> void:
	rotate_y(deg_to_rad(rotation_speed_deg) * delta)
	
func _physics_process(delta: float) -> void:
	for body in area_3d.get_overlapping_bodies():
		if body is Spaceship:
			print("spaceship entered", body.ship_id)

			# Gravitační síla
			var dir: Vector3 = global_transform.origin - body.global_transform.origin
			var dist: float = dir.length()
			var force: Vector3 = dir.normalized() * gravity_strength * (1.0 - dist / gravity_range)
			body.set_gravity_force(force * -1)

			# Rotace kolem středu planety
			var center = global_transform.origin
			var rot_dir = body.global_transform.origin - center
			var angle = deg_to_rad(rotation_speed_deg) * delta
			rot_dir = rot_dir.rotated(Vector3.UP, angle)
			body.global_transform.origin = center + rot_dir

			# Natočení lodi podle rotace planety
			body.rotate_y(angle)
	
func save_faces_as_resources() -> void:
	for face in faces:
		var res: PlanetLODData = face_mesh_resolution[face.mesh_id]
		var path = "Scenes/Space/Space Generation/Planet/Planet_1/Meshes/planet_%d_face_%d.tres" % [planet_id, face.mesh_id]
		var err = ResourceSaver.save(res, path)
		if err != OK:
			push_error("Failed to save resource: " + path)
			
func load_faces_from_resources() -> void:
	for i in range(6):
		var path = "Scenes/Space/Space Generation/Planet/Planet_1/Meshes/planet_%d_face_%d.tres" % [planet_id, i]
		if ResourceLoader.exists(path):
			face_mesh_resolution[i] = load(path)

func _on_area_3d_body_exited(body: Node3D) -> void:
	if body is Spaceship:
			print("spaceship left", body.ship_id)
			(body as Spaceship).set_gravity_force(Vector3(0.0, 0.0, 0.0))
			

func _on_close_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	current_resolution = CLOSE_RESOLUTION
	print("applying new mesh: ", current_resolution)
	apply_mesh(CLOSE_RESOLUTION)


func _on_close_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
	
	current_resolution = MID_RESOLUTION
	print("applying new mesh: ", current_resolution)
	apply_mesh(MID_RESOLUTION)


func _on_far_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if current_resolution == FAR_RESOLUTION:
		return
	current_resolution = FAR_RESOLUTION
	print("applying new mesh: ", current_resolution)
	apply_mesh(FAR_RESOLUTION)

func _on_far_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if current_resolution == OUT_RESOLUTION:
		return
	current_resolution = OUT_RESOLUTION
	print("applying new mesh: ", current_resolution)
	apply_mesh(OUT_RESOLUTION)


func _on_mid_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if current_resolution == MID_RESOLUTION:
		return
	current_resolution = MID_RESOLUTION
	print("applying new mesh: ", current_resolution)
	apply_mesh(MID_RESOLUTION)


func _on_mid_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if current_resolution == FAR_RESOLUTION:
		return
	current_resolution = FAR_RESOLUTION
	print("applying new mesh: ", current_resolution)
	apply_mesh(FAR_RESOLUTION)
