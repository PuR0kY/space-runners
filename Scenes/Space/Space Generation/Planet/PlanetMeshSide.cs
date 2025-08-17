using Godot;
using Godot.NativeInterop;
using System;

[Tool]
public partial class PlanetMeshFace : MeshInstance3D
{
	[Export] public int MeshId { get; set; }
	[Export] public Vector3 Normal { get; set; }
	[Export] public Material Material { get; set; }

	private Planet Owner { get; set; }

	public override void _Ready()
	{
		// Případně by se tu mohl načítat materiál jen jednou
	}

	public Godot.Collections.Array RegenerateMesh(int resolution)
	{
		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		int numVertices = resolution * resolution;
		int numIndices = (resolution - 1) * (resolution - 1) * 6;

		Vector3[] vertexArray = [];
		Vector2[] uvArray = [];
		Vector3[] normalArray = [];
		int[] indexArray = [];

		int triIndex = 0;
		Vector3 axisA = new Vector3(Normal.Y, Normal.Z, Normal.X);
		Vector3 axisB = Normal.Cross(axisA);

		float MAX_HEIGHT = 0.0f;
		float MIN_HEIGHT = 99999.0f;

		for (int y = 0; y < resolution; y++)
		{
			for (int x = 0; x < resolution; x++)
			{
				int i = x + y * resolution;
				Vector2 percent = new Vector2((float)x, (float)y) / (resolution - 1);
				Vector3 pointOnUnitCube = Normal +
										  (percent.X - 0.5f) * 2f * axisA +
										  (percent.Y - 0.5f) * 2f * axisB;
				Vector3 pointOnUnitSphere = pointOnUnitCube.Normalized();
				Vector3 pointOnPlanet = Owner.PointOnPlanet(pointOnUnitSphere);

				vertexArray[i] = pointOnPlanet;

				float l = pointOnPlanet.Length();
				if (l < MIN_HEIGHT)
					MIN_HEIGHT = l;
				if (l > 0)
					MAX_HEIGHT = l;

				if (x != resolution - 1 && y != resolution - 1)
				{
					indexArray[triIndex + 2] = i;
					indexArray[triIndex + 1] = i + resolution + 1;
					indexArray[triIndex] = i + resolution;

					indexArray[triIndex + 5] = i;
					indexArray[triIndex + 4] = i + 1;
					indexArray[triIndex + 3] = i + resolution + 1;
					triIndex += 6;
				}
			}
		}

		for (int a = 0; a < indexArray.Length; a += 3)
		{
			int ia = indexArray[a];
			int ib = indexArray[a + 1];
			int ic = indexArray[a + 2];

			Vector3 ab = vertexArray[ib] - vertexArray[ia];
			Vector3 bc = vertexArray[ic] - vertexArray[ib];
			Vector3 ca = vertexArray[ia] - vertexArray[ic];

			Vector3 n = (ab.Cross(bc) + bc.Cross(ca) + ca.Cross(ab)) * -1.0f;

			normalArray[ia] += n;
			normalArray[ib] += n;
			normalArray[ic] += n;
		}

		for (int i = 0; i < normalArray.Length; i++)
		{
			normalArray[i] = normalArray[i].Normalized();
		}

		arrays[(int)Mesh.ArrayType.Vertex] = vertexArray;
		arrays[(int)Mesh.ArrayType.Normal] = normalArray;
		arrays[(int)Mesh.ArrayType.TexUV] = uvArray;
		arrays[(int)Mesh.ArrayType.Index] = indexArray;

		return arrays;
	}

	public void UpdateMesh(Godot.Collections.Array arrays)
	{
		var mesh = new ArrayMesh();
		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		mesh.SurfaceSetMaterial(0, Material);
		this.Mesh = mesh;
	}
}
