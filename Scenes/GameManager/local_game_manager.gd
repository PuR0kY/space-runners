# LocalGameManager.gd (autoload)
extends Node

var selected_ship_id := ""
var player_name := ""
var ship_config := {} # name → ship data

func _ready():
	load_ship_config()
	
func pick_ship(name: String):
	if ship_config.has(name):
		selected_ship_id = name

func load_ship_config():
	var file = FileAccess.open("res://Gameplay/spaceships.json", FileAccess.READ)
	if file:
		var text = file.get_as_text()
		var data = JSON.parse_string(text)
		ship_config = data
