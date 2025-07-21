extends Node3D

const ROT_SPEED = 2

func _process(_delta: float) -> void:
	rotate_y(deg_to_rad(ROT_SPEED))

func _on_area_3d_body_entered(body: Node3D) -> void:
	if body is Spaceship:
		var spaceship = body as Spaceship
		spaceship.add_orb()
		queue_free()
