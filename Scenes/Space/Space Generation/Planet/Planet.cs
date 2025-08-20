using Godot;

public partial class Planet : Node3D
{
    [Export] public int Resolution = 320;
    [Export] public FastNoiseLite[] Noises;
    [Export] public Material Material;
    [Export] public float Radius;

    private PlanetMeshFace[] _faces;

    public override void _Ready()
    {
        _faces = new PlanetMeshFace[6];

        _faces[0] = new PlanetMeshFace(this, Resolution, Vector3.Up, Material, Noises, Radius);
        _faces[1] = new PlanetMeshFace(this, Resolution, Vector3.Down, Material, Noises, Radius);
        _faces[2] = new PlanetMeshFace(this, Resolution, Vector3.Left, Material, Noises, Radius);
        _faces[3] = new PlanetMeshFace(this, Resolution, Vector3.Right, Material, Noises, Radius);
        _faces[4] = new PlanetMeshFace(this, Resolution, Vector3.Forward, Material, Noises, Radius);
        _faces[5] = new PlanetMeshFace(this, Resolution, Vector3.Back, Material, Noises, Radius);

        var area = GetNode<Node3D>("Areas");

        area.Connect("spaceship_entered", new Callable(this, nameof(OnSpaceshipEntered)));

        GeneratePlanet();
    }

    public void OnSpaceshipEntered(int resolution)
    {
        GD.Print("Spaceship entered! Setting resolution to: " + resolution);
        // TODO: Regen only faces that are visible for player
        foreach (var face in _faces)
        {
            face.SetResolution(resolution);
            face.Generate();
        }
    }

    private void GeneratePlanet()
    {
        foreach (var face in _faces)
        {
            face.Generate();
        }
    }
}
