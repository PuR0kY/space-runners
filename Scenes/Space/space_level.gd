extends Node3D

@export var player_scene: PackedScene
@onready var players_holder: Node3D = $PlayersHolder
var instanced_players: Dictionary = {}
#func _ready():
	#pass
#
#func start_multiplayer():
	#var keys = nakama_manager.players.keys()
	#keys.sort()
	#for i in keys:
		#var instanced_player = player_scene.instantiate()
		#instanced_player.set_multiplayer_authority(i)
		#
		#var session = nakama_manager.session
		#var user_id = nakama_manager.session.user_id
		#
		#instanced_player.name = str(nakama_manager.players[i].name)
		#var requests = [
			#NakamaStorageObjectId.new("ships", "selected_ship", user_id)
		#]
#
		#var result = await nakama_manager.client.read_storage_objects_async(session, requests)
		#
		#var storage_obj = result.objects[0]
		#var json_string = storage_obj.value
		## Parse JSON string back to Dictionary or your ship data type
		#var json = JSON.new()
		#var ship_data = json.parse_string(json_string)
		#instanced_player.spaceships_config = ship_data
#
		#players_holder.add_child(instanced_player)
		#instanced_player.global_position = Vector3(randf_range(0.0, 100.0), randf_range(0.0, 100.0), randf_range(0.0, 100.0))
#
#func start_freeplay():
	#pass
	##if not is_multiplayer_authority():
		##return
##
	##var keys = NakamaMultiplayer.players.keys()
	##keys.sort()
	##var instanced_player = player_scene.instantiate()
	##instanced_player.set_multiplayer_authority(keys[0])
	##players_holder.add_child(instanced_player)
	##instanced_player.global_position = Vector3(randf_range(0.0, 100.0), randf_range(0.0, 100.0), randf_range(0.0, 100.0))


func _ready():
