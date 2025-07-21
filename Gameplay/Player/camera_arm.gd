extends SpringArm3D

@export var mouse_sensitivity: float = 0.005
var camera_zoom_levels = [5.0, 15.0] # 1st person 0.05 not working properly yet , 3rd person (close), 3rd person (far)
var zoom_index := 1  # Start at medium (5.0)

func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	
func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		rotation.y -= event.relative.x * mouse_sensitivity
		
		# First person needs to be clamped so it cant look inside seat
		if zoom_index == 0:
			rotation.y = clamp(rotation.y, -PI/4, PI/4)
		
		rotation.x -= event.relative.y * mouse_sensitivity 
		rotation.x = clamp(rotation.x, -PI/8, PI/2)

	if event.is_action_pressed("wheel_up"):
		zoom_index = min(zoom_index + 1, camera_zoom_levels.size() - 1)
	elif event.is_action_pressed("wheel_down"):
		zoom_index = max(zoom_index - 1, 1)
	
	spring_length = camera_zoom_levels[zoom_index]

		
	print(spring_length)
		
	if event.is_action_pressed("toggle_mouse_capture"):
		if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
