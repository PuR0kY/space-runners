@tool
class_name PlanetMeshFace
extends MeshInstance3D

@export var mesh_id: int
@export var normal: Vector3

var material = load("res://Scenes/Space/Space Generation/Planet/materials/Planet_1_material.tres") as Material

func regenerate_mesh(_resolution: int, planet_data: PlanetData) -> Array:
	var arrays := []
	arrays.resize(Mesh.ARRAY_MAX)

	var vertex_array := PackedVector3Array()
	var uv_array := PackedVector2Array()
	var normal_array := PackedVector3Array()
	var index_array := PackedInt32Array()

	var resolution: float = (_resolution)
	var num_vertices: int = resolution * resolution
	var num_indices: int = (resolution - 1) * (resolution - 1) * 6

	normal_array.resize(num_vertices)
	uv_array.resize(num_vertices)
	vertex_array.resize(num_vertices)
	index_array.resize(num_indices)

	var tri_index: int = 0
	var axisA := Vector3(normal.y, normal.z, normal.x)
	var axisB: Vector3 = normal.cross(axisA)

	for y in range(resolution):
		for x in range(resolution):
			var i: int = x + y * resolution
			var percent := Vector2(x, y) / (resolution - 1)
			var pointOnUnitCube: Vector3 = normal + (percent.x - 0.5) * 2.0 * axisA + (percent.y - 0.5) * 2.0 * axisB
			var pointOnUnitSphere := pointOnUnitCube.normalized()
			var pointOnPlanet := planet_data.point_on_planet(pointOnUnitSphere)
			vertex_array[i] = pointOnPlanet
			
			var l = pointOnPlanet.length()
			if l < planet_data.min_height:
				planet_data.min_height = l
			if l > planet_data.max_height:
				planet_data.max_height = l

			if x != resolution - 1 and y != resolution - 1:
				index_array[tri_index+2] = i
				index_array[tri_index+1] = i+resolution+1
				index_array[tri_index] = i+resolution

				index_array[tri_index+5] = i
				index_array[tri_index+4] = i+1
				index_array[tri_index+3] = i+resolution+1
				tri_index += 6

	for a in range(0, index_array.size(), 3):
		var ia = index_array[a]
		var ib = index_array[a + 1]
		var ic = index_array[a + 2]

		var ab = vertex_array[ib] - vertex_array[ia]
		var bc = vertex_array[ic] - vertex_array[ib]
		var ca = vertex_array[ia] - vertex_array[ic]

		var n = (ab.cross(bc) + bc.cross(ca) + ca.cross(ab)) * -1.0

		normal_array[ia] += n
		normal_array[ib] += n
		normal_array[ic] += n

	for i in range(normal_array.size()):
		normal_array[i] = normal_array[i].normalized()

	arrays[Mesh.ARRAY_VERTEX] = vertex_array
	arrays[Mesh.ARRAY_NORMAL] = normal_array
	arrays[Mesh.ARRAY_TEX_UV] = uv_array
	arrays[Mesh.ARRAY_INDEX] = index_array
	
	return arrays

func update_mesh(planet_data: PlanetData, arrays: Array):
	var _mesh := ArrayMesh.new()
	_mesh.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, arrays)
	self.mesh = _mesh
	
	material.set_shader_parameter("min_height", planet_data.min_height)
	material.set_shader_parameter("max_height", planet_data.max_height)
	material.set_shader_parameter("height_color", planet_data.planet_color)

	_mesh.surface_set_material(0, material)
