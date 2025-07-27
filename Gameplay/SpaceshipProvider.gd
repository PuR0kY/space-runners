extends Node

var file = FileAccess.open("res://Gameplay/spaceships.json", FileAccess.READ)
var json_string = file.get_as_text()
var spaceships = JSON.parse_string(json_string)

var selected_ship
