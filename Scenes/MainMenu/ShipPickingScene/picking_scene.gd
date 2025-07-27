extends Node2D
@onready var grid_container: GridContainer = $Spaceships/GridContainer
# Load the file content as a string
# health
@onready var health_value: ProgressBar = $Spaceships/StatsPanel/health/Value
@onready var dmg_value: ProgressBar = $Spaceships/StatsPanel/dmg/Value
@onready var speed_value: ProgressBar = $Spaceships/StatsPanel/max_speed/Value
@onready var handling_value: ProgressBar = $Spaceships/StatsPanel/handling/Value
@onready var fire_rate_value: ProgressBar = $Spaceships/StatsPanel/fire_rate/Value
@export var pick_button: Button
@onready var ship: Node3D = $"../Ship"

signal Spawn

func _ready() -> void:
	if not is_multiplayer_authority():
		hide()

	var item = preload("res://Scenes/MainMenu/ShipPickingScene/spaceship_card.tscn")
	var spaceships = SpaceshipProvider.spaceships
	
	if spaceships is Dictionary:	
		for spaceship in spaceships:
			var card = item.instantiate()
			grid_container.add_child(card)
			card.set_data(spaceships[spaceship], spaceship)
			card.onSelected.connect(set_stats)
			
func set_stats(data: Dictionary, name: String) -> void:
	pick_button.text = "Pick " + name + "!"
	SpaceshipProvider.selected_ship = name
	print(SpaceshipProvider.selected_ship, " selected!")

	health_value.value = data["health"]
	dmg_value.value = data["damage"]
	speed_value.value = data["max_speed"]
	handling_value.value = data["handling"]
	fire_rate_value.value = data["fire_rate"]
	spawn_ship_mesh(data)

func spawn_ship_mesh(data: Dictionary) -> void:
	if ship.get_child_count() > 0:
		var nodes = ship.get_children(true)
		for node in nodes:
			node.queue_free()

	var scene_path = data["ship_scene_path"]
	var scene = load(scene_path)
	var instance = scene.instantiate()
	ship.add_child(instance)

func on_pick_click() -> void:
	hide()
