extends Control
@onready var grid_container: GridContainer = $SpaceshipList
# Load the file content as a string
# health
@onready var health_value: ProgressBar = $StatsPanel/health/Value
@onready var dmg_value: ProgressBar = $StatsPanel/dmg/Value
@onready var speed_value: ProgressBar = $StatsPanel/max_speed/Value
@onready var handling_value: ProgressBar = $StatsPanel/handling/Value
@onready var fire_rate_value: ProgressBar = $StatsPanel/fire_rate/Value
@onready var ship: Node3D = $"../Ship"

@onready var stat_bars := {
	"health": health_value,
	"damage": dmg_value,
	"max_speed": speed_value,
	"handling": handling_value,
	"fire_rate": fire_rate_value
}

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
			
			
func smooth_set_progressbar(bar: ProgressBar, new_value: float, duration := 0.3):
	var t := create_tween()
	t.tween_property(bar, "value", new_value, duration).set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)

func set_stats(data: Dictionary, name: String) -> void:
	SpaceshipProvider.selected_ship = name
	print(SpaceshipProvider.selected_ship, " selected!")

	for stat_name in stat_bars.keys():
		if data.has(stat_name):
			smooth_set_progressbar(stat_bars[stat_name], data[stat_name])

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
