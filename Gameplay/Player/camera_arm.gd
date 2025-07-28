class_name CameraArm
extends SpringArm3D

@export var mouse_sensitivity: float = 0.005
var camera_zoom_levels = [5.0, 15.0] # 1st person 0.05 not working properly yet , 3rd person (close), 3rd person (far)
var zoom_index := 1  # Start at medium (5.0)
var combat_mode := false
@onready var camera: Camera3D = $Camera
@export var aim_sensitivity: float = 80.0

func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	
func toggle_combat_mode():
	combat_mode = !combat_mode
	var t = create_tween()
	if combat_mode:
		t.tween_property(self, "spring_length", -2.0, 0.4)
		t.tween_property(camera, "fov", 100, 0.4)
		zoom_index = 0
	else:
		zoom_index = 1
		t.tween_property(self, "spring_length", camera_zoom_levels[zoom_index], 0.4)
		t.tween_property(camera, "fov", 80, 0.4)
	
func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("combat_mode"):
		toggle_combat_mode()
	
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		rotation.y -= event.relative.x * mouse_sensitivity
		rotation.x -= event.relative.y * mouse_sensitivity 
		# First person needs to be clamped so it cant look inside seat
		if zoom_index == 0:
			rotation.y = clamp(rotation.y, -PI/12, PI/12)
			rotation.x = clamp(rotation.x, -PI/16, PI/8)
		else:
			rotation.x = clamp(rotation.x, -PI/8, PI/2)
		
	if event.is_action_pressed("toggle_mouse_capture"):
		if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
