using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noises
{
    private static Vector2[] points;

    public static Texture2D GenerateVoronoiNoise(int textureWidth, int textureHeight, int voronoiPoints,Vector2[] pointsList = null)
    {
        Texture2D tex = new Texture2D(textureWidth, textureHeight);


        if(pointsList == null){
            //Random Points 
            points = new Vector2[voronoiPoints];
            for (int i = 0; i < voronoiPoints; i++)
            {
                var x = UnityEngine.Random.Range(0.0f, 1.1f);
                var y = UnityEngine.Random.Range(0.0f, 1.1f);
                points[i] = new Vector2(x, y);
            }
        }
        else{
            points = pointsList;
        }
        for (int i = 0; i < textureWidth; i++)
        {
            for (int o = 0; o < textureHeight; o++)
            {
                var coords = new Vector2((float)i / textureWidth, (float)o / textureHeight);
                var distances = new float[points.Length];
                for (int p = 0; p < points.Length; p++)
                {
                    distances[p] = Vector2.Distance(coords, points[p]);
                }
                System.Array.Sort(distances);
                var nth = 0;
                var noiseValue = distances[nth];
                var randomPixel = new Color(noiseValue, noiseValue, noiseValue);
                tex.SetPixel(i, o, randomPixel);
            }
        }
        tex.Apply();
        return tex;
    }

    internal static Vector2[] GetVoronoiPoints()
    {
        return points;
    }

    internal static void GenerateVoronoiNoise(object textureWidth, object textureHeight, object points)
    {
        throw new NotImplementedException();
    }

    public static Texture2D GeneratePerlinNoise(int textureWidth, int textureHeight, float noiseScale, Vector2 noiseOffset)
    {
        Texture2D tex = new Texture2D(textureWidth, textureHeight);
        for (int i = 0; i < textureWidth; i++)
        {
            for (int o = 0; o < textureHeight; o++)
            {

                var noiseValue = Mathf.PerlinNoise(((float)i / textureWidth) * noiseScale + noiseOffset.x, ((float)o / textureHeight) * noiseScale + noiseOffset.y);
                var randomPixel = new Color(noiseValue, noiseValue, noiseValue);
                tex.SetPixel(i, o, randomPixel);
            }
        }
        tex.Apply();
        return tex;
    }
}
