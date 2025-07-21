class_name Projectile extends Node3D

const SPEED = 40.0
@export var ship_speed: float
@export var explosion_vfx: PackedScene
@onready var mesh: MeshInstance3D = $Mesh
@onready var ray_cast_3d: RayCast3D = $RayCast3D
@onready var gpu_particles_3d: GPUParticles3D = $GPUParticles3D


func _process(delta: float) -> void:
	position += transform.basis * Vector3(0, 0, -(200.0 if !ship_speed else ship_speed * 2.0)) * delta
	if ray_cast_3d.is_colliding():
		mesh.visible = false
		gpu_particles_3d.emitting = true
		var explosion = explosion_vfx.instantiate()
		explosion.position = ray_cast_3d.global_position
		explosion.transform.basis = ray_cast_3d.global_transform.basis
		get_parent().add_child(explosion)
		await get_tree().create_timer(1.0).timeout
		queue_free()


func _on_timer_timeout() -> void:
	queue_free()
