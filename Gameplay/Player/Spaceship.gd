class_name Spaceship extends CharacterBody3D

# Match stats
var orb_count = 0
var credit_count = 0
var has_atomic_missile := false
var is_buy_menu_opened = false

# Constant values
var pitch_speed = 1.5
var roll_speed = 1.9
var yaw_speed = 1.25

# Values to be loaded from JSON
var health = 100.0
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

var projectile = load("res://Gameplay/Shooting/projectile.tscn")
var bullet: Projectile

@onready var mesh: Node3D = $Mesh
@onready var camera: Camera3D = $CameraOffset/Camera
@onready var raycast: RayCast3D = $RayCast
@onready var camera_offset: SpringArm3D = $CameraOffset
@onready var visor: Visor = $SubViewportContainer/SubViewport/Node2D as Visor

# Player Header UI
@onready var player_tag: Sprite3D = $PlayerTag
@onready var health_bar: ProgressBar = $PlayerTag/SubViewport/HealthBar
@onready var player_name: Label = $PlayerTag/SubViewport/PlayerName

var ship_id = SpaceshipProvider.selected_ship

func _ready() -> void:
	GDSync.connect_gdsync_owner_changed(self, owner_changed)
	GDSync.expose_func(set_mesh)
	GDSync.expose_func(shoot)
	GDSync.expose_func(deal_damage)

func owner_changed(_owner_id: int) -> void:
	var isOwner := GDSync.is_gdsync_owner(self)
	camera.current = isOwner
	visor.visible = isOwner

	if isOwner:
		player_tag.visible = false # we dont want to see our own header
		update_hud()
		set_mesh(SpaceshipProvider.spaceships[ship_id])
		GDSync.call_func(set_mesh, [SpaceshipProvider.spaceships[ship_id]])
	
func set_mesh(config: Dictionary) -> void:
	health = config["health"]
	damage = config["damage"]
	magazine_size = config["magazine_size"]
	fire_rate = config["fire_rate"]
	max_speed = config["max_speed"]
	acceleration = config["acceleration"]
	input_response = config["handling"]

	var scene = load(config["ship_scene_path"])
	GDSync.multiplayer_instantiate((scene as PackedScene), mesh, true, [], true)
	mesh_offset_transform = mesh.transform
	
	# player tag setup
	# TODO: Do budoucna asi budou muset být dva,
	# jeden zelený a jeden červený a podle nějakého příznaku isEnemy zobrazovat jeden nebo druhý
	health_bar.max_value = health
	health_bar.value = health
	player_name.text = ship_id
	
	# Visor update
	visor.set_health(health, true)

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
	
func _process(_delta: float) -> void:
	if !GDSync.is_gdsync_owner(self):
		return;

	mesh.global_transform = global_transform * mesh_offset_transform
	visor.set_speed(forward_speed)
	
	if Input.is_action_just_pressed("quit"):
		get_tree().quit()
		
	# Only local authority controls the ship
	if Input.is_action_just_pressed("shoot"):
		GDSync.call_func(shoot)

func shoot():
	bullet = GDSync.multiplayer_instantiate(projectile, get_parent(), true, [], true)
	bullet.ship_speed = forward_speed
	print("Setting damage of value: ", damage, " to projectile")
	bullet.damage = damage
	# TODO: Raycast should be determined of ship type. Some ships are much bigger
	bullet.position = raycast.global_position
	bullet.transform.basis = raycast.global_transform.basis
	
func send_damage_dealt(dmg: int) -> void:
	GDSync.call_func(deal_damage, [dmg], true)
	
func deal_damage(dmg: int) -> void:
	# TODO: Shield logic
	health -= dmg
	visor.set_health(health, false)
	health_bar.value = health
	# TODO: Kill logic

func _physics_process(delta: float) -> void:
	if GDSync.is_gdsync_owner(self):
		get_input(delta)
		
		# Rotation
		transform.basis = transform.basis.rotated(transform.basis.z.normalized(), roll_input * roll_speed * delta)
		transform.basis = transform.basis.rotated(transform.basis.x.normalized(), pitch_input * pitch_speed * delta)
		transform.basis = transform.basis.rotated(transform.basis.y.normalized(), yaw_input * yaw_speed * delta)
		transform.basis = transform.basis.orthonormalized()

		# Calculate velocity and move
		velocity = -transform.basis.z.normalized() * forward_speed
		move_and_slide()
