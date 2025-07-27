class_name Projectile extends Node3D

const SPEED = 80.0
@export var damage = 20.0
@export var ship_speed: float
@export var explosion_vfx: PackedScene
@onready var mesh: MeshInstance3D = $Mesh
@onready var ray_cast_3d: RayCast3D = $RayCast3D
@onready var gpu_particles_3d: GPUParticles3D = $GPUParticles3D

var hit_count = 0

func _physics_process(delta: float) -> void:
	position += transform.basis * Vector3(0, 0, -(200.0 if !ship_speed else ship_speed * 2.0)) * delta

func _process(delta: float) -> void:
	if ray_cast_3d.is_colliding() && hit_count == 0:
		var collided_object = ray_cast_3d.get_collider()
		collided_object.send_damage_dealt(damage)
		hit_count += 1
		print("dealt damage of: ", damage, "to spaceship: ", collided_object)
		gpu_particles_3d.emitting = true
		queue_free()
		
		# TODO: Explosion VFX fix
		#var explosion = explosion_vfx.instantiate()
		#explosion.position = ray_cast_3d.global_position
		#explosion.transform.basis = ray_cast_3d.global_transform.basis
		#get_parent().add_child(explosion)


func _on_timer_timeout() -> void:
	queue_free()
