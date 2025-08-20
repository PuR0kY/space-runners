using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class PlanetMeshFace
{
    private PlanetLOD _lod;
    private MeshInstance3D _meshInstance;
    private Vector3 _localUp;
    private Material _material;

    private int _resolution;

    public PlanetMeshFace(Node parent, int resolution, Vector3 localUp, Material material, FastNoiseLite[] noises, float radius)
    {
        var highFreqNoise = new FastNoiseLite();
        highFreqNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;

        _resolution = resolution;
        _lod = new PlanetLOD(_resolution, noises);
        _lod.Radius = radius;
        _localUp = localUp;
        _material = material;

        _meshInstance = new MeshInstance3D();
        parent.AddChild(_meshInstance);
    }

    public void SetResolution(int resolution) => this._resolution = resolution;

    public void Generate()
    {
        _lod.Generate(_localUp);
        _meshInstance.Mesh = _lod.Mesh;
        _meshInstance.SetSurfaceOverrideMaterial(0, _material);
    }
}
