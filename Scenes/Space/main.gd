extends Node3D

func _ready() -> void:
	GDSync.connected.connect(connected)
	GDSync.connection_failed.connect(connection_failed)
	GDSync.lobby_created.connect(lobby_created)
	GDSync.lobby_creation_failed.connect(lobby_create_failed)
	GDSync.lobby_joined.connect(lobby_joined)
	GDSync.lobby_join_failed.connect(lobby_join_failed)
	
func connected() -> void:
	print("Connected!")
	GDSync.lobby_create("TestLobby")
	pass
	
func connection_failed(error) -> void:
	pass
	
func lobby_created(lobby_name: String) -> void:
	print("Lobby ", lobby_name, " Created!")
	GDSync.lobby_join(lobby_name)
	
func lobby_joined(lobby_name: String) -> void:
	print("Lobby ", lobby_name, " joined!")
	
func lobby_create_failed(lobby_name, err) -> void:
	print("Failed to create lobby ", lobby_name, err)
	
	if err == ENUMS.LOBBY_CREATION_ERROR.LOBBY_ALREADY_EXISTS:
		GDSync.lobby_join(lobby_name)
	
func lobby_join_failed(lobby_name, err) -> void:
	print("Failed to join lobby ", lobby_name, err)
