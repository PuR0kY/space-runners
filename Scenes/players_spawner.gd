extends Node3D

var spaceship = load("res://Gameplay/Player/Spaceship.tscn")

func _ready() -> void:
	GDSync.client_joined.connect(client_joined)
	GDSync.client_left.connect(client_left)
	
func client_joined(client_id: int) -> void:
	print("Client ", client_id, " joined")
	var player = (spaceship as PackedScene).instantiate()
	add_child(player)
	player.name = str(client_id)
	GDSync.set_gdsync_owner(player, client_id)
	
func client_left(client_id: int) -> void:
	pass
