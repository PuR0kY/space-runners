@tool
extends Node3D
class_name Planet

@export var planet_data: PlanetData
@export var rotation_speed_deg: float = 1.0 # stupně za sekundu
@export var gravity_strength: float = 20.0  # m/s^2
@export var gravity_range: float = 1000.0       # maximální dosah gravitačního pole
@onready var area_3d: Area3D = $Area3D
var planet_id: int = 1

const CLOSE_RESOLUTION = 650
const MID_RESOLUTION = 350
const FAR_RESOLUTION = 150
const OUT_RESOLUTION = 50

# Funkce, která vrátí novou instanci slovníku LOD
func get_lod_meshes() -> Dictionary:
	return {
		OUT_RESOLUTION: [],
		FAR_RESOLUTION: [],
		MID_RESOLUTION: [],
		CLOSE_RESOLUTION: []
	}

var face_mesh_resolution = {
	0: get_lod_meshes(),
	1: get_lod_meshes(),
	2: get_lod_meshes(),
	3: get_lod_meshes(),
	4: get_lod_meshes(),
	5: get_lod_meshes()
	}
	
var faces: Array[PlanetMeshFace] = []
var threads: Array[Thread] = []
	
func _ready() -> void:
	for child in get_children():
		if child is PlanetMeshFace:
			faces.append(child)

	if planet_data:
		for n in planet_data.planet_noises:
			if n.noise_map and not n.noise_map.is_connected("changed", on_data_changed):
				n.noise_map.changed.connect(on_data_changed)
			area_3d.monitorable = true
			area_3d.monitoring = true
			planet_data.resolution = OUT_RESOLUTION
	generate_lod_async(OUT_RESOLUTION)
	generate_lod_async(FAR_RESOLUTION)
	generate_lod_async(MID_RESOLUTION)
	generate_lod_async(CLOSE_RESOLUTION)
	apply_mesh(OUT_RESOLUTION)
	save_faces_as_json()
		
func generate_lod_async(resolution: int) -> void:
	for child in faces:
		if child is PlanetMeshFace:
			print("creating mesh for face: ", child.mesh_id)
			print("with resolution: ", resolution)
			face_mesh_resolution[child.mesh_id][resolution] = child.regenerate_mesh(resolution, planet_data)

func _create_meshes(faces: Array[PlanetMeshFace], planet_data: PlanetData, resolution: int) -> void:
	for face in faces:
		print("creating mesh for face: ", face.mesh_id)
		print("with resolution: ", resolution)
		face_mesh_resolution[face.mesh_id][resolution] = face.regenerate_mesh(resolution, planet_data)

func apply_mesh(resolution: int) -> void:
	for child in faces:
		if child is PlanetMeshFace:
			child.update_mesh(planet_data, face_mesh_resolution[child.mesh_id][resolution])
	
func _process(delta: float) -> void:
	rotate_y(deg_to_rad(rotation_speed_deg) * delta)
	 # Sledujeme vlákna v hlavním cyklu
	var threads_to_remove: Array[Thread] = []
	for thread in threads:
		if not thread.is_alive():
			threads_to_remove.append(thread)
	
	for thread in threads_to_remove:
		thread.wait_to_finish() # Abychom se ujistili, že vlákno je opravdu pryč
		threads.erase(thread)
	
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

func on_data_changed():
	if not planet_data:
		return
	planet_data.min_height = 99999.0
	planet_data.max_height = 0.0
	
func save_faces_as_json() -> void:
	for face in faces:
		var path = "F:/space-runners/space-runners/planet_" + str(planet_id) + "_" + str(face.mesh_id) + "_" + ".json"
		var json_string = JSON.stringify(face_mesh_resolution[face.mesh_id])
		var file = FileAccess.open(path, FileAccess.WRITE)
		file.store_string(json_string)
		file.close()
		print("Planet config saved")

func _on_area_3d_body_exited(body: Node3D) -> void:
	if body is Spaceship:
			print("spaceship left", body.ship_id)
			(body as Spaceship).set_gravity_force(Vector3(0.0, 0.0, 0.0))
			

func _on_close_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if planet_data.resolution == CLOSE_RESOLUTION:
		return
	planet_data.resolution = CLOSE_RESOLUTION
	print("applying new mesh: ", planet_data.resolution)
	apply_mesh(CLOSE_RESOLUTION)


func _on_close_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if planet_data.resolution == MID_RESOLUTION:
		return
	
	planet_data.resolution = MID_RESOLUTION
	print("applying new mesh: ", planet_data.resolution)
	apply_mesh(MID_RESOLUTION)


func _on_far_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if planet_data.resolution == FAR_RESOLUTION:
		return
	planet_data.resolution = FAR_RESOLUTION
	print("applying new mesh: ", planet_data.resolution)
	apply_mesh(FAR_RESOLUTION)

func _on_far_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if planet_data.resolution == OUT_RESOLUTION:
		return
	planet_data.resolution = OUT_RESOLUTION
	print("applying new mesh: ", planet_data.resolution)
	apply_mesh(OUT_RESOLUTION)


func _on_mid_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if planet_data.resolution == MID_RESOLUTION:
		return
	planet_data.resolution = MID_RESOLUTION
	print("applying new mesh: ", planet_data.resolution)
	apply_mesh(MID_RESOLUTION)


func _on_mid_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
		
	if planet_data.resolution == FAR_RESOLUTION:
		return
	planet_data.resolution = FAR_RESOLUTION
	print("applying new mesh: ", planet_data.resolution)
	apply_mesh(FAR_RESOLUTION)
