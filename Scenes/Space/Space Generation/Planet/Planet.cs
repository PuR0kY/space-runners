using Godot;
using SpacePirates.Scenes.Space.Space_Generation.Planet;
using SpacePirates.Scenes.Space.Space_Generation.Planet.LOD;
using System;
using System.Collections.Generic;
using System.IO;

[Tool]
public partial class Planet : Node3D
{
	[Export] int ID { get; set; }
	[Export] string Name { get; set; }
	[Export] public float Radius { get; set; }
	[Export] public GradientTexture1D PlanetColor { get; set; }
	[Export] public NoiseTexture2D[] PlanetNoises { get; set; }
	[Export] public float Amplitude { get; set; }
	[Export] public float MinHeight { get; set; }
	[Export] public bool IsFirstLayerMask { get; set; }
	[Export] public float Resolution { get; set; }

	[Export] public float RotationSpeedDeg { get; set; } = 1.0f;   // deg/s
	[Export] public float GravityStrength { get; set; } = 20.0f;   // m/s^2
	[Export] public float GravityRange { get; set; } = 1000.0f;    // max dosah gravitačního pole
	[Export] public NodePath Area3DPath { get; set; }
	[Export] public bool GenerateNewMeshes { get; set; } = false;

	private Area3D _area3D;

	public Dictionary<int, PlanetLOD> FaceMeshResolution = new()
		{
			{ 0, new PlanetLOD() },
			{ 1, new PlanetLOD() },
			{ 2, new PlanetLOD() },
			{ 3, new PlanetLOD() },
			{ 4, new PlanetLOD() },
			{ 5, new PlanetLOD() }
		};

	private readonly List<PlanetMeshFace> _faces = new();

	public override void _Ready()
	{
		// najdi Area3D
		if (Area3DPath != null && !Area3DPath.IsEmpty)
			_area3D = GetNode<Area3D>(Area3DPath);

		// posbírej face nody
		foreach (var child in GetChildren())
		{
			if (child is PlanetMeshFace face)
			{
				_faces.Add(face);
			}
		}

		// GENERATING NEW PLANET MESHES
		var path = $"res://Scenes/Space/Space Generation/Planet/Planet_1/Meshes/planet_{ID}_face_{ID}.tres";
		if (GenerateNewMeshes)
		{
			GenerateLod(Shared.OUT_RESOLUTION);
			GenerateLod(Shared.FAR_RESOLUTION);
			GenerateLod(Shared.MID_RESOLUTION);
			GenerateLod(Shared.MID_RESOLUTION);
			SaveFacesAsResources();
		}

		// LOADING EXISTING MESHES
		LoadFacesFromResources();
		ApplyMesh(Shared.OUT_RESOLUTION);

		var areas = GetNode<Node>("Areas");
		if (areas != null)
		{
			areas.Connect("spaceship_entered", new Callable(this, nameof(ApplyMesh)));
		}
	}
	

	public Vector3 PointOnPlanet(Vector3 pointOnSphere)
	{
		float elevation = 0.0f;
		float baseElevation = 0.0f;

		if (PlanetNoises.Length > 0)
			baseElevation = Shared.GetElevation(PlanetNoises[0], pointOnSphere, Amplitude, MinHeight);

		foreach (var noise in PlanetNoises)
		{
			float mask = 1.0f;
			if (IsFirstLayerMask)
				mask = baseElevation;

			var levelElevation = Shared.GetElevation(noise, pointOnSphere, Amplitude, MinHeight) * mask;
			elevation += levelElevation;
		}

		return pointOnSphere * Radius * (elevation + 1.0f);
	}

	// --------------------
	// LOD generování & aplikace
	// --------------------
	private void GenerateLod(float resolution)
	{
		foreach (var face in _faces)
		{
			GD.Print($"creating mesh for face: {face.MeshId}");
			GD.Print($"with resolution: {resolution}");
			var arrays = face.RegenerateMesh(resolution);
			FaceMeshResolution[face.MeshId].LODs[(int)resolution] = arrays;
		}
	}

	private void ApplyMesh(float resolution)
	{
		foreach (var face in _faces)
		{
			GD.Print($"applying mesh for face: {face.MeshId}");
			var arrays = FaceMeshResolution[face.MeshId].GetLod((int)resolution);
			if (arrays.Count == 0)
			{
				GD.PrintErr("No mesh arrays to apply for face ", face.MeshId, " at resolution ", resolution);
				return; // skip updating
			}
			face.UpdateMesh(arrays);
		}
	}

	public override void _Process(double delta)
	{
		RotateY(Mathf.DegToRad(RotationSpeedDeg) * (float)delta);
	}

	// --------------------
	// Ukládání / načítání Resource s face LODy
	// --------------------
	private void SaveFacesAsResources()
	{
		foreach (var face in _faces)
		{
			var res = FaceMeshResolution[face.MeshId];
			var path = $"res://Scenes/Space/Space Generation/Planet/Planet_1/Meshes/planet_{ID}_face_{face.MeshId}.tres";
			var err = ResourceSaver.Save(res, path);
			if (err != Error.Ok)
				GD.PushError("Failed to save resource: " + path);
		}
	}

	private void LoadFacesFromResources()
	{
		foreach (var face in _faces)
		{
			var path = $"res://Scenes/Space/Space Generation/Planet/Planet_1/Meshes/planet_{1}_face_{face.MeshId}.tres";
			GD.Print("Path of Resource exists: " + ResourceLoader.Exists(path));
			if (ResourceLoader.Exists(path))
			{
				var loaded = ResourceLoader.Load<PlanetLOD>(path);
				GD.Print("Loaded:" + loaded != null);

				if (loaded != null)
					FaceMeshResolution[face.MeshId] = loaded;
			}
		}
	}
}
