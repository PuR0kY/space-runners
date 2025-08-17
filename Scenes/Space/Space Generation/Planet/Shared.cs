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
        float baseElevation = 0.0f;
        baseElevation = noise.Noise.GetNoise3Dv(pointOnSphere * 100.0f);
        baseElevation = ((baseElevation + 1.0f) / 2.0f) * amplitude;
        baseElevation = Math.Max(0.0f, baseElevation - minHeight);
        return baseElevation;
    }

    public static float CLOSE_RESOLUTION = 650.0f;
    public static float MID_RESOLUTION = 350.0f;
    public static float FAR_RESOLUTION = 150.0f;
    public static float OUT_RESOLUTION = 5.0f;
}
