extends Node3D

var spaceship = load("res://Gameplay/Player/Spaceship.tscn")

func _ready() -> void:
	GDSync.client_joined.connect(client_joined)
	
func client_joined(client_id: int) -> void:
	print("Client ", client_id, " joined")
	var player = (spaceship as PackedScene).instantiate()
	add_child(player)
	GDSync.set_gdsync_owner(player, client_id)
