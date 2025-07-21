extends Node3D

@export var player_scene: PackedScene
@onready var players_holder: Node3D = $PlayersHolder
var instanced_players: Dictionary = {}

func _ready():
	pass
