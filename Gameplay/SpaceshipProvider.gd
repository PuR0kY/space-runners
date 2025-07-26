extends Node

var file = FileAccess.open("res://Gameplay/spaceships.json", FileAccess.READ)
var json_string = file.get_as_text()
var spaceships_object = JSON.new()

var selected_ship: String
var spaceships = spaceships_object.parse_string(json_string)
