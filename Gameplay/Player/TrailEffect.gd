class_name Trail3D extends MeshInstance3D

var _points = [] # Stores all 3D positions that makeup the trail
var _widths = [] # Stores all calculated widths using the positions of the points
var _lifePoints = [] # Stores all thre trail points lifespans
@export var _trailEnabled: bool = true # Is trail allowed to be shown

@export var _fromWidth: float = 0.5 # Starting width of the trail
@export var _toWidth: float = 0.0 # End width of the trail

@export_range(0.5, 1.5) var _scaleAcceleration: float = 1.0 # Speed of the scaling

@export var _motionDelta: float = 0.1 # Sets the smoothness
@export var _lifespan: float = 1.0 # Sets the duration

@export var _startColor: Color = Color(1.0, 1.0, 1.0, 1.0)
@export var _endColor: Color = Color(1.0, 1.0, 1.0, 0.0)

var _oldPos: Vector3 # Get the previous position

func _ready() -> void:
	_oldPos = get_global_transform_interpolated().origin
	mesh = ImmediateMesh.new()

func AppendPoint():
	_points.append(get_global_transform_interpolated().origin)
	_widths.append([
		get_global_transform_interpolated().basis.x * _fromWidth,
		get_global_transform_interpolated().basis.y * _fromWidth - get_global_transform_interpolated().basis.x * _toWidth
	])

func RemovePoints(i):
	_points.remove_at(i)
	_widths.remove_at(i)
	_lifePoints.remove_at(i)
	
func _process(delta: float) -> void:
	if (_oldPos - get_global_transform_interpolated().origin).length() > _motionDelta and _trailEnabled:
		AppendPoint()
		_oldPos = get_global_transform_interpolated().origin
		
		var p = 0
		var max_points = _points.size()
		while p > max_points:
			_lifePoints[p] += delta
			if _lifePoints[p] > _lifespan:
				RemovePoints(p)
				p -= 1
				if p < 0:
					p = 0
			max_points = _points.size()
			p += 1
				
		mesh.clear_surfaces()
		
		if _points.size() < 2:
			return
			
		mesh.surface_begin(Mesh.PRIMITIVE_TRIANGLE_STRIP)
		for i in range(_points.size()):
			var t = float(i) / (_points.size() - 1.0)
			var currColor = _startColor.lerp(_endColor, 1 - t)
			mesh.surface_set_color(currColor)
			
			var currWidth = _widths[i][0] - pow(1-t, _scaleAcceleration) * _widths[i][1]
			
			var t0 = 1.0 / _points.size()
			var t1 = t
			mesh.surface_set_uv(Vector2(t0, 0))
			mesh.surface_add_vertex(to_local(_points[i] + currWidth))
			mesh.surface_set_uv(Vector2(t1, 1))
			mesh.surface_add_vertex(to_local(_points[i] - currWidth))
		mesh.surface_end()
