using Godot;
using SpacePirates.Scenes.Space.Space_Generation.Planet;
using SpacePirates.Scenes.Space.Space_Generation.Planet.LOD;
using System;
using System.Collections.Generic;

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

	public Dictionary<int, PlanetLOD> FaceMeshResolution = PlanetLOD.GetFaceMeshResolution();

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
				_faces.Add(face);
		}

		// GENERATING NEW PLANET MESHES
		if (GenerateNewMeshes)
		{
			GenerateLod(Shared.OUT_RESOLUTION);
			GenerateLod(Shared.FAR_RESOLUTION);
			GenerateLod(Shared.MID_RESOLUTION);
			GenerateLod(Shared.CLOSE_RESOLUTION);
			SaveFacesAsResources();
		}

		// LOADING EXISTING MESHES
		LoadFacesFromResources();
		ApplyMesh(Shared.OUT_RESOLUTION);
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

			var levelElevation = Shared.GetElevation(noise, pointOnSphere, Amplitude, MinHeight);
			elevation += levelElevation;
		}

		return pointOnSphere * Radius * (elevation + 1.0f);
	}

	// --------------------
	// LOD generování & aplikace
	// --------------------
	private void GenerateLod(int resolution)
	{
		foreach (var face in _faces)
		{
			GD.Print($"creating mesh for face: {face.MeshId}");
			GD.Print($"with resolution: {resolution}");
			var arrays = face.RegenerateMesh(resolution);
			FaceMeshResolution[face.MeshId].SetLod(resolution, arrays);
		}
	}

	private void ApplyMesh(int resolution)
	{
		foreach (var face in _faces)
		{
			var arrays = FaceMeshResolution[face.MeshId].GetLod(resolution);
			face.UpdateMesh(arrays);
		}
	}

	public override void _Process(double delta)
	{
		RotateY(Mathf.DegToRad(RotationSpeedDeg) * (float)delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_area3D == null)
			return;

		var bodies = _area3D.GetOverlappingBodies();
		foreach (var b in bodies)
		{
			if (b is Spaceship ship)
			{
				GD.Print($"spaceship entered {ship.ship_id}");

				var dir = GlobalTransform.Origin - ship.GlobalTransform.Origin;
				float dist = dir.Length();
				var force = dir.Normalized() * GravityStrength * (1.0f - dist / GravityRange);
				ship.SetGravityForce(force * -1f);

				// Rotace kolem středu planety
				var center = GlobalTransform.Origin;
				var rotDir = ship.GlobalTransform.Origin - center;
				float angle = Mathf.DegToRad(RotationSpeedDeg) * (float)delta;
				rotDir = rotDir.Rotated(Vector3.Up, angle);
				ship.GlobalTransform = new Transform3D(
					ship.GlobalTransform.Basis,
					center + rotDir
				);

				// Natočení lodi podle rotace planety
				ship.RotateY(angle);
			}
		}
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
		for (int i = 0; i < 6; i++)
		{
			var path = $"res://Scenes/Space/Space Generation/Planet/Planet_1/Meshes/planet_{ID}_face_{i}.tres";
			if (ResourceLoader.Exists(path))
			{
				var loaded = ResourceLoader.Load<PlanetLOD>(path);
				if (loaded != null)
					FaceMeshResolution[i] = loaded;
			}
		}
	}

	// --------------------
	// Handlery pro signály z oblastí (připoj v editoru)
	// --------------------
	public void OnArea3DBodyExited(Node3D body)
	{
		if (body is Spaceship ship)
		{
			GD.Print($"spaceship left {ship.ship_id}");
			ship.SetGravityForce(Vector3.Zero);
		}
	}

	public void OnCloseAreaBodyEntered(Node3D body)
	{
		if (body is not Spaceship) return;

		Resolution = Shared.CLOSE_RESOLUTION;
		GD.Print($"applying new mesh: {Resolution}");
		ApplyMesh(Shared.CLOSE_RESOLUTION);
	}

	public void OnCloseAreaBodyExited(Node3D body)
	{
		if (body is not Spaceship) return;

		Resolution = Shared.MID_RESOLUTION;
		GD.Print($"applying new mesh: {Resolution}");
		ApplyMesh(Shared.MID_RESOLUTION);
	}

	public void OnFarAreaBodyEntered(Node3D body)
	{
		if (body is not Spaceship) return;
		if (Resolution == Shared.FAR_RESOLUTION) return;

		Resolution = Shared.FAR_RESOLUTION;
		GD.Print($"applying new mesh: {Resolution}");
		ApplyMesh(Shared.FAR_RESOLUTION);
	}

	public void OnFarAreaBodyExited(Node3D body)
	{
		if (body is not Spaceship) return;
		if (Resolution == Shared.OUT_RESOLUTION) return;

		Resolution = Shared.OUT_RESOLUTION;
		GD.Print($"applying new mesh: {Resolution}");
		ApplyMesh(Shared.OUT_RESOLUTION);
	}

	public void OnMidAreaBodyEntered(Node3D body)
	{
		if (body is not Spaceship) return;
		if (Resolution == Shared.MID_RESOLUTION) return;

		Resolution = Shared.MID_RESOLUTION;
		GD.Print($"applying new mesh: {Resolution}");
		ApplyMesh(Shared.MID_RESOLUTION);
	}

	public void OnMidAreaBodyExited(Node3D body)
	{
		if (body is not Spaceship) return;
		if (Resolution == Shared.FAR_RESOLUTION) return;

		Resolution = Shared.FAR_RESOLUTION;
		GD.Print($"applying new mesh: {Resolution}");
		ApplyMesh(Shared.FAR_RESOLUTION);
	}
}
