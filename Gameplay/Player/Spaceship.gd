class_name Spaceship extends CharacterBody3D

var orb_count = 0
var credit_count = 0
var has_atomic_missile := false
var is_buy_menu_opened = false

# Constant values
var pitch_speed = 1.5
var roll_speed = 1.9
var yaw_speed = 1.25

# Values to be loaded from JSON
var health
var damage
var magazine_size
var fire_rate
var max_speed
var acceleration
var input_response = 1.0
var mesh_offset_transform: Transform3D

# Runtime values
var ship_velocity = Vector3.ZERO
var forward_speed = 0.0
var pitch_input = 0.0
var roll_input = 0.0
var yaw_input = 0.0

var target_basis = Basis()
var target_position = Vector3()
var sync_timer := 0.0

@onready var mesh: Node3D = $Mesh

var projectile = load("res://Gameplay/Shooting/projectile.tscn")
var instance
var spaceships_config

@onready var camera: Camera3D = $CameraOffset/Camera
@onready var raycast: RayCast3D = $RayCast
@onready var camera_offset: SpringArm3D = $CameraOffset
@onready var visor: Visor = $SubViewportContainer/SubViewport/Node2D as Visor

func _ready() -> void:
	GDSync.connect_gdsync_owner_changed(self, owner_changed)
	spaceships_config = SpaceshipProvider.spaceships[SpaceshipProvider.selected_ship]
		
func owner_changed(owner_id: int) -> void:
	var isOwner := GDSync.is_gdsync_owner(self)
	camera.current = isOwner
	$SubViewportContainer.visible = isOwner

	if isOwner:
		var selected_ship_id = SpaceshipProvider.selected_ship
		rpc("_broadcast_ship_id", selected_ship_id)
		set_mesh(SpaceshipProvider.spaceships[selected_ship_id])
		update_hud()
	
func set_mesh(config: Dictionary) -> void:
	health = config["health"]
	damage = config["damage"]
	magazine_size = config["magazine_size"]
	fire_rate = config["fire_rate"]
	max_speed = config["max_speed"]
	acceleration = config["acceleration"]
	input_response = config["handling"]

	var scene = load(config["ship_scene_path"])
	var ship = (scene as PackedScene).instantiate()
	mesh.add_child(ship)
	mesh_offset_transform = mesh.transform
	
@rpc("any_peer", "call_local", "reliable")
func _broadcast_ship_id(ship_id: String) -> void:
	var sender_id = get_multiplayer().get_remote_sender_id()
	if sender_id == multiplayer.get_unique_id():
		return  # Je to zpráva od nás, ignoruj

	if not SpaceshipProvider.spaceships.has(ship_id):
		push_warning("Unknown ship_id from peer: " + ship_id)
		return

	var config = SpaceshipProvider.spaceships[ship_id]
	set_mesh(config)

func get_input(delta):
	# Buy menu
	if Input.is_action_just_pressed("buy_action"):
		is_buy_menu_opened = !is_buy_menu_opened # negace ( "!" ) : přehodí hodnotu na opačnou
		visor.buy_menu.visible = is_buy_menu_opened

		if is_buy_menu_opened:
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
		
	
	# Movement
	if Input.is_action_pressed("move_forward"):
		forward_speed = clamp(forward_speed + acceleration * delta, 0, max_speed)
		if forward_speed > max_speed * 0.5 and forward_speed <= max_speed:
			camera.fov = lerp(camera.fov, 100.0, delta * 2.0)
	if Input.is_action_pressed("move_backward"):
		forward_speed = clamp(forward_speed - acceleration * delta, 0, max_speed)
		camera.fov = lerp(camera.fov, 80.0, delta * 2.0)  # Smoothly return to normal
		
	roll_input = lerp(
		roll_input,
		Input.get_action_raw_strength("roll_left") - Input.get_action_raw_strength("roll_right"),
		input_response * delta)
		
	yaw_input = lerp(
		yaw_input,
		Input.get_action_raw_strength("turn_left") - Input.get_action_raw_strength("turn_right"),
		input_response * delta)
		
	pitch_input = lerp(
		pitch_input,
		Input.get_action_raw_strength("pitch_up") - Input.get_action_raw_strength("pitch_down"),
		input_response * delta)
		
func add_orb() -> void:
	orb_count += 1
	visor.set_orb_count(orb_count)
	
func update_hud() -> void:
	visor.set_orb_count(orb_count)
	visor.set_credits_count(credit_count)
	visor.set_speed(forward_speed)
	
func _process(delta: float) -> void:
	if !GDSync.is_gdsync_owner(self):
		return;
		
	var fps = Engine.get_frames_per_second()
	var lerp_interval = velocity / fps
	var lerp_position = global_transform.origin + lerp_interval

	mesh.global_transform = global_transform * mesh_offset_transform
	visor.set_speed(forward_speed)
	
	if Input.is_action_just_pressed("quit"):
		get_tree().quit()
		
@rpc("any_peer", "call_local", "reliable")
func _shoot():
	instance = projectile.instantiate()
	instance.ship_speed = forward_speed
	instance.position = raycast.global_position
	instance.transform.basis = raycast.global_transform.basis
	get_parent().add_child(instance)

func _physics_process(delta: float) -> void:
	if GDSync.is_gdsync_owner(self):
		# Only local authority controls the ship
		if Input.is_action_just_pressed("shoot"):
			rpc("_shoot")
		
		get_input(delta)
		
		# Rotation
		transform.basis = transform.basis.rotated(transform.basis.z.normalized(), roll_input * roll_speed * delta)
		transform.basis = transform.basis.rotated(transform.basis.x.normalized(), pitch_input * pitch_speed * delta)
		transform.basis = transform.basis.rotated(transform.basis.y.normalized(), yaw_input * yaw_speed * delta)
		transform.basis = transform.basis.orthonormalized()

		# Calculate velocity and move
		velocity = -transform.basis.z.normalized() * forward_speed
		move_and_slide()

		# Sync every 100ms
		sync_timer += delta
		if sync_timer > 0.1:
			sync_timer = 0.0
			rpc("sync_position", global_transform.basis, global_transform.origin, velocity)

	if !GDSync.is_gdsync_owner(self):
		global_transform.basis = global_transform.basis.slerp(target_basis.orthonormalized(), delta * 5.0).orthonormalized()
		global_transform.origin = global_transform.origin.lerp(target_position, delta * 5.0)
		return
	
	
@rpc("authority", "call_remote", "unreliable")
func sync_position(new_basis: Basis, new_position: Vector3, new_velocity: Vector3):
	if GDSync.is_gdsync_owner(self):
		return

	target_basis = new_basis
	target_position = new_position
	velocity = new_velocity
