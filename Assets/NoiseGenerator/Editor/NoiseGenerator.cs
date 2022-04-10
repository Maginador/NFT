using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class NoiseGenerator : EditorWindow
{
    public enum NoiseType { SimpleNoise, PerlinNoise, VoronoiNoise };
    public NoiseType currentNoise;

    int textureWidth, textureHeight;
    int oldTextureWidth, oldTextureHeight;

    string textureName;
    bool autoGenerate = false;
    float noiseScale = 5;
    Vector2 noiseOffset = new Vector2(0, 0);
    float oldNoiseScale = 5;
    Vector2 oldNoiseOffset = new Vector2(0, 0);

    //Voronoi 
    int voronoiPoints = 5;
    int oldVoronoiPoints;
    Texture2D generatedTexture;
    private bool parametersChanged;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Noise Generator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        NoiseGenerator window = (NoiseGenerator)EditorWindow.GetWindow(typeof(NoiseGenerator));
        window.Show();
    }

    void OnGUI()
    {
        parametersChanged = false;
        currentNoise = (NoiseType)EditorGUILayout.EnumPopup(currentNoise);
        textureWidth = EditorGUILayout.IntField("Texture Width", textureWidth);
        textureHeight = EditorGUILayout.IntField("Texture Height", textureHeight);
        if (textureHeight != oldTextureHeight)
        {
            parametersChanged = true;
            oldTextureHeight = textureHeight;
        }
        if (textureWidth != oldTextureWidth)
        {
            parametersChanged = true;
            oldTextureWidth = textureWidth;
        }
        switch (currentNoise)
        {
            case NoiseType.PerlinNoise:
                PerlinNoiseWindow();
                break;
            case NoiseType.VoronoiNoise:
                VoronoiNoiseWindow();
                break;
        }
        if (noiseScale != oldNoiseScale)
        {
            parametersChanged = true;
            oldNoiseScale = noiseScale;
        }
        if (noiseOffset != oldNoiseOffset)
        {
            parametersChanged = true;
            oldNoiseOffset = noiseOffset;
        }
        if (voronoiPoints != oldVoronoiPoints)
        {
            parametersChanged = true;
            if (voronoiPoints <= 0) voronoiPoints = 1;
            oldVoronoiPoints = voronoiPoints;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Automatically Generate Texture");
        autoGenerate = EditorGUILayout.Toggle(autoGenerate);
        GUILayout.EndHorizontal();

        if (autoGenerate)
        {
            if (parametersChanged)
            {
                generatedTexture = GenerateNoiseTexture();
            }
        }
        else
        {
            if (GUILayout.Button("Generate Texture"))
            {
                generatedTexture = GenerateNoiseTexture();
            }
        }
        GUILayout.Box(generatedTexture);
        textureName = EditorGUILayout.TextField(textureName);
        if (GUILayout.Button("Create Texture"))
        {
            CreateTexture();
        }
    }

    private void CreateTexture()
    {
        byte[] bytes = generatedTexture.EncodeToPNG();
        if (!Directory.Exists(Application.dataPath + "/ExportedNoise/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ExportedNoise/");
        }
        File.WriteAllBytes(Application.dataPath + "/ExportedNoise/" + textureName + ".png", bytes);
        AssetDatabase.Refresh();
    }


    ///Gradient windows
    private void PerlinNoiseWindow()
    {

        noiseScale = EditorGUILayout.FloatField("Noise Scale", noiseScale);

        noiseOffset = EditorGUILayout.Vector2Field("Noise Offset", noiseOffset);
    }

    private void VoronoiNoiseWindow()
    {
        voronoiPoints = EditorGUILayout.IntField("Number of Points", voronoiPoints);
    }


    ///Noise Generation

    public Texture2D GenerateNoiseTexture()
    {

        switch (currentNoise)
        {
            case NoiseType.SimpleNoise:
                return GenerateSimpleNoise();
            case NoiseType.PerlinNoise:
                return Noises.GeneratePerlinNoise(textureWidth, textureHeight, noiseScale, noiseOffset);
            case NoiseType.VoronoiNoise:
                return Noises.GenerateVoronoiNoise(textureWidth,textureHeight,voronoiPoints);
        }
        return null;
    }


    ///Gradient Generation Algorithms
    private Texture2D GenerateSimpleNoise()
    {
        Texture2D tex = new Texture2D(textureWidth, textureHeight);
        for (int i = 0; i < textureWidth; i++)
        {
            for (int o = 0; o < textureHeight; o++)
            {
                var randomValue = UnityEngine.Random.Range(0.0f, 1.1f);
                var randomPixel = new Color(randomValue, randomValue, randomValue);
                tex.SetPixel(i, o, randomPixel);
            }
        }
        tex.Apply();
        return tex;
    }
    public Texture2D GenerateDoubleVerticalInterpolatedGrad()
    {

        Texture2D tex = new Texture2D(textureWidth, textureHeight);

        tex.Apply();
        return tex;
    }
}