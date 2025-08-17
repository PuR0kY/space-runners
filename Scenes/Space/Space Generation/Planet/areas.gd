extends Node3D

signal spaceship_entered(resolution: int)

@export var area_3d: Area3D
@export var gravity_strength: float
@export var gravity_range: float
@export var rotation_speed_deg: float

const CLOSE_RESOLUTION = 650
const MID_RESOLUTION = 350
const FAR_RESOLUTION = 150
const OUT_RESOLUTION = 5

func _physics_process(delta: float) -> void:
	if not area_3d:
		return

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

func _on_area_3d_body_exited(body: Node3D) -> void:
	if body is Spaceship:
			print("spaceship left", body.ship_id)
			(body as Spaceship).set_gravity_force(Vector3(0.0, 0.0, 0.0))
			

func _on_close_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
	emit_signal("spaceship_entered", CLOSE_RESOLUTION)


func _on_close_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
	emit_signal("spaceship_entered", MID_RESOLUTION)


func _on_far_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
	emit_signal("spaceship_entered", FAR_RESOLUTION)

func _on_far_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;	
	emit_signal("spaceship_entered", OUT_RESOLUTION)


func _on_mid_area_body_entered(body: Node3D) -> void:
	if body is not Spaceship:
		return;
	emit_signal("spaceship_entered", MID_RESOLUTION)


func _on_mid_area_body_exited(body: Node3D) -> void:
	if body is not Spaceship:
		return;
	emit_signal("spaceship_entered", FAR_RESOLUTION)


func _on_area_3d_body_entered(body: Node3D) -> void:
	pass # Replace with function body.
