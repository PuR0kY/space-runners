class_name Visor extends Node2D
@onready var credits: Label = $Credits
@onready var speed: Label = $Speed
@onready var orbs: Label = $Orbs
@onready var buy_menu: Panel = $BuyMenu
@onready var health_bar: ProgressBar = $VisorOverlay/HealthBar

@export var player: CharacterBody3D
var prev_rotation: Vector2

func _ready() -> void:
	prev_rotation = Vector2(player.rotation.y, player.rotation.x)
	buy_menu.visible = false
	
func set_health(value: int, max_value: bool) -> void:
	health_bar.value = value
	
	if max_value:
		health_bar.max_value = value
	
func set_orb_count(count: int) -> void:
	orbs.text = "orbs: %d" % count

func set_credits_count(count: int) -> void:
	credits.text = "credits: %d" % count
	
func set_speed(count: float) -> void:
	speed.text = "speed: %d" % count
	
func _physics_process(delta: float) -> void:
	var current_rotation := Vector2(player.rotation.y, player.rotation.x)
	var dist := current_rotation - prev_rotation
	$Camera2D.offset -= dist * delta * 1600.0
	prev_rotation = current_rotation

	$Camera2D.offset = lerp($Camera2D.offset, Vector2.ZERO, 1.0 - pow(0.03, delta))
