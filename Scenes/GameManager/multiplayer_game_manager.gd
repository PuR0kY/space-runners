# MultiplayerGameManager.gd
extends Node

var player_data := {} # player_id -> { "name": ..., "ship_id": ... }
var spaceships
@onready var players: Node3D = $Players
@export var space_scene: PackedScene
@onready var is_host := multiplayer.is_server()

func _ready():
	if is_host:
		# Ask all clients to send their local ship pick and name
		rpc("send_ship_choice_request")
		var file = FileAccess.open("res://Gameplay/spaceships.json", FileAccess.READ)
		var json_string = file.get_as_text()
		var spaceships_object = JSON.new()
		spaceships = spaceships_object.parse_string(json_string) as Dictionary
		create_space()

# Ask each client to send their local ship data
@rpc("any_peer")
func send_ship_choice_request():
	var id = multiplayer.get_unique_id()
	var name = LocalGameManager.player_name
	var ship = LocalGameManager.selected_ship_id
	rpc_id(1, "submit_ship_choice", id, ship, name)

# Host collects data from clients
@rpc("authority")
func submit_ship_choice(player_id: int, ship_id: String, name: String):
	player_data[player_id] = {
		"ship_id": ship_id,
		"name": name,
	}

	# Optional: once all connected players are collected
	if player_data.size() == multiplayer.get_peers().size() + 1: # +1 for host
		rpc("finalize_player_data", player_data)

@rpc("any_peer")
func create_space():
	var space = space_scene.instantiate()
	add_child(space)

# Sync to all clients
@rpc("any_peer")
func finalize_player_data(data: Dictionary):
	player_data = data
	spawn_all_players()

func spawn_all_players():
	for player_id in player_data.keys():
		var ship_id = player_data[player_id]["ship_id"]
		spawn_ship(player_id, ship_id)
		
func load_ship_config():
	var file = FileAccess.open("res://Gameplay/spaceships.json", FileAccess.READ)
	if file:
		var text = file.get_as_text()
		var data = JSON.parse_string(text)
		spaceships = data

func spawn_ship(player_id: int, ship_name: String):
	var ship_data = LocalGameManager.ship_config.get(ship_name)
	if ship_data == null:
		push_error("Ship not found: " + ship_name)
		return

	var scene_path = ship_data.ship_scene_path
	var scene = load(scene_path)
	var ship = scene.instantiate()

	ship.set_multiplayer_authority(player_id)
	players.add_child(ship) 
	apply_ship_stats(ship, ship_data)
	
func apply_ship_stats(ship, data):
	ship.health = data.health
	ship.damage = data.damage
	ship.fire_rate = data.fire_rate
	ship.max_speed = data.max_speed
	ship.acceleration = data.acceleration
	ship.handling = data.handling
