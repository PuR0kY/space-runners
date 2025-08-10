class_name Visor extends Node2D
@onready var credits: Label = $Credits
@onready var speed: Label = $Speed
@onready var orbs: Label = $Orbs
@onready var buy_menu: Panel = $BuyMenu
@onready var health_bar: ProgressBar = $VisorOverlay/HealthBar
@onready var crosshair: TextureRect = $Crosshair
@onready var speed_lines: ColorRect = $Camera2D/Control/SpeedLines
const TRANSPARENT_SPEED_LINES_COLOR = "ffffff00"
@export var player: Spaceship
var prev_rotation: Vector2

var speed_lines_material
var speed_lines_color

func _ready() -> void:
	prev_rotation = Vector2(player.rotation.y, player.rotation.x)
	buy_menu.visible = false
	
	# Material initial setup
	speed_lines_material = speed_lines.material as ShaderMaterial
	if speed_lines_material:
		speed_lines_color = speed_lines_material.get_shader_parameter("line_color") as Color
		speed_lines_color.a = 0.0  # Or whatever alpha you want
		speed_lines_material.set_shader_parameter("line_color", speed_lines_color)
	
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
	
	if player.max_speed:
		var alpha = clamp(player.forward_speed / player.max_speed, 0.0, 1.0)
		var color: Color = speed_lines_material.get_shader_parameter("line_color")
		color.a = alpha
		speed_lines_material.set_shader_parameter("line_color", color)

	$Camera2D.offset = lerp($Camera2D.offset, Vector2.ZERO, 1.0 - pow(0.03, delta))
