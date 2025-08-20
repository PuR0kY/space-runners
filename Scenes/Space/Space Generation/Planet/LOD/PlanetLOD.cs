using Godot;
using System.Collections.Generic;

public class PlanetLOD
{
	public int Resolution { get; }
	public ArrayMesh Mesh { get; private set; }

	public float Radius { get; set; }

	private FastNoiseLite[] _noiseLayers;

	public PlanetLOD(int resolution, FastNoiseLite[] noiseLayers)
	{
		Resolution = resolution;
		_noiseLayers = noiseLayers;
	}

	public void Generate(Vector3 localUp)
	{
		// 2 pomocné osy (pro grid)
		Vector3 axisA = new Vector3(localUp.Y, localUp.Z, localUp.X);
		Vector3 axisB = localUp.Cross(axisA);

		int numVertices = Resolution * Resolution;
		var vertices = new Vector3[numVertices];
		var normals = new Vector3[numVertices];
		var uvs = new Vector2[numVertices];
		var indices = new int[(Resolution - 1) * (Resolution - 1) * 6];

		int i = 0;
		for (int y = 0; y < Resolution; y++)
		{
			for (int x = 0; x < Resolution; x++)
			{
				float xf = (x / (float)(Resolution - 1) - 0.5f) * 2f;
				float yf = (y / (float)(Resolution - 1) - 0.5f) * 2f;

				Vector3 pointOnCube = localUp + xf * axisA + yf * axisB;
				Vector3 pointOnSphere = pointOnCube.Normalized();

				// Noise pro elevation
				Vector3 v = pointOnSphere.Normalized();

				float elevation = 0f;

				foreach (var noise in _noiseLayers)
				{
					float scale = 1f / (noise.Frequency);
					elevation += noise.GetNoise3D(pointOnCube.X * scale, pointOnCube.Y * scale, pointOnCube.Z * scale) * (Radius / 2f);
				}

				float radius = Radius + elevation * 0.2f;

				v *= radius;

				vertices[i] = v;
				normals[i] = pointOnSphere;
				uvs[i] = new Vector2(x / (float)(Resolution - 1), y / (float)(Resolution - 1));

				i++;
			}
		}

		// Triangulace
		int t = 0;
		for (int y = 0; y < Resolution - 1; y++)
		{
			for (int x = 0; x < Resolution - 1; x++)
			{
				int i0 = x + y * Resolution;
				int i1 = x + (y + 1) * Resolution;
				int i2 = (x + 1) + y * Resolution;
				int i3 = (x + 1) + (y + 1) * Resolution;

				indices[t++] = i0;
				indices[t++] = i1;
				indices[t++] = i2;

				indices[t++] = i2;
				indices[t++] = i1;
				indices[t++] = i3;
			}
		}

		// ArrayMesh naplnění
		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)ArrayMesh.ArrayType.Max);
		arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
		arrays[(int)ArrayMesh.ArrayType.Normal] = normals;
		arrays[(int)ArrayMesh.ArrayType.TexUV] = uvs;
		arrays[(int)ArrayMesh.ArrayType.Index] = indices;

		var mesh = new ArrayMesh();
		mesh.AddSurfaceFromArrays(ArrayMesh.PrimitiveType.Triangles, arrays);
		Mesh = mesh;
	}
}
