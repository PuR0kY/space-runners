using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpacePirates.Scenes.Space.Space_Generation.Planet;

public static class Shared
{
    public static float GetElevation(NoiseTexture2D noise, Godot.Vector3 pointOnSphere, float amplitude, float minHeight)
    {
        var baseElevation = 0.0f;
        baseElevation = noise.Noise.GetNoise3Dv(pointOnSphere * 100.0f);
        baseElevation = (baseElevation + 1f) / 2.0f * amplitude;
        baseElevation = Math.Max(0, baseElevation - minHeight);
        return baseElevation;
    }

    public static int CLOSE_RESOLUTION = 650;
    public static int MID_RESOLUTION = 350;
    public static int FAR_RESOLUTION = 150;
    public static int OUT_RESOLUTION = 5;
}
