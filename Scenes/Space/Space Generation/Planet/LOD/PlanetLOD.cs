using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace SpacePirates.Scenes.Space.Space_Generation.Planet.LOD;

[Tool]
public partial class PlanetLOD : Resource
{
    [Export] public int MeshId { get; set; }
    [Export] public Material Material { get; set; }

    private Dictionary<int, Godot.Collections.Array> _lods = new();

    public void SetLod(int resolution, Godot.Collections.Array arrays)
    {
        _lods[resolution] = arrays;
    }

    public Godot.Collections.Array GetLod(int resolution)
    {
        if (_lods.TryGetValue(resolution, out var arrays))
            return arrays;

        return new Godot.Collections.Array();
    }

    public static Dictionary<int, PlanetLOD> GetFaceMeshResolution()
    {
        return new()
        {
            { 0, new PlanetLOD() },
            { 1, new PlanetLOD() },
            { 2, new PlanetLOD() },
            { 3, new PlanetLOD() },
            { 4, new PlanetLOD() },
            { 5, new PlanetLOD() }
        };
    }
}
