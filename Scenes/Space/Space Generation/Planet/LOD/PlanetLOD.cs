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

    [Export]
    public Godot.Collections.Dictionary<int, Godot.Collections.Array> LODs = new();


    public void SetLod(int resolution, Godot.Collections.Array arrays)
    {
        LODs[resolution] = arrays;
    }

    public Godot.Collections.Array GetLod(int resolution)
    {
        GD.Print(LODs.Count);
        if (LODs.TryGetValue(resolution, out var arrays))
            return arrays;

        return [];
    }
}
