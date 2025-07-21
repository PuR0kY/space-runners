extends Node3D

## Transfers current spaceship's orbs to credits
func transfer_orbs_to_credits(spaceship: Spaceship) -> void:

	## early return
	if spaceship.orb_count == 0:
		return;

 	# Maybe some kind of multiplier that can be upgraded in future
	spaceship.credit_count += spaceship.orb_count * 2
	spaceship.orb_count = 0
	spaceship.update_hud()

func _on_area_3d_body_entered(body: Node3D) -> void:
	if body is Spaceship:
		var spaceship = body as Spaceship
		transfer_orbs_to_credits(spaceship)
