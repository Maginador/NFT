using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class GradientGenerator : EditorWindow
{    
    int textureWidth, textureHeight;
    string textureName;
    Gradient verticalGradient = new Gradient();
    Gradient verticalGradient2 = new Gradient();

    public int keys { get; private set; }
    public int keys2 { get; private set; }

    Gradient horizontalGradient = new Gradient();
    float intensity = 0.5f;
    Texture2D generatedTexture;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Gradient Generator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GradientGenerator window = (GradientGenerator)EditorWindow.GetWindow(typeof(GradientGenerator));
        window.Show();
    }

    void OnGUI()
    {
        textureWidth = EditorGUILayout.IntField(textureWidth);
        textureHeight = EditorGUILayout.IntField(textureHeight);

        GUILayout.BeginHorizontal();
        verticalGradient = EditorGUILayout.GradientField("Gradient", verticalGradient);
        keys = EditorGUILayout.IntSlider(keys,2,8);

        if(GUILayout.Button("Random Gradient")){
            verticalGradient = GenerateGradient(keys);
        }
        GUILayout.EndHorizontal();
         GUILayout.BeginHorizontal();
        verticalGradient2 = EditorGUILayout.GradientField("Gradient", verticalGradient2);
        keys = EditorGUILayout.IntSlider(keys,2,8);

        if(GUILayout.Button("Random Gradient")){
            verticalGradient2 = GenerateGradient(keys);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        horizontalGradient = EditorGUILayout.GradientField("Gradient", horizontalGradient);
        keys2 = EditorGUILayout.IntSlider(keys2,2,8);

        if(GUILayout.Button("Random Gradient")){
            horizontalGradient = GenerateGradient(keys2, true);
        }
        GUILayout.EndHorizontal();
        intensity = EditorGUILayout.FloatField(intensity);
        if(GUILayout.Button("GenTexture")) generatedTexture = 
        GenerateGradTexture(verticalGradient,verticalGradient2,horizontalGradient);
        GUILayout.Box(generatedTexture);
        textureName = EditorGUILayout.TextField(textureName);
        if(GUILayout.Button("Create Texture")){
            byte[] bytes = generatedTexture.EncodeToPNG();
            if(!Directory.Exists(Application.dataPath + "/ExportedGradient/")){
                Directory.CreateDirectory(Application.dataPath + "/ExportedGradient/");
            }
            File.WriteAllBytes(Application.dataPath + "/ExportedGradient/" + textureName+ ".png", bytes);
            AssetDatabase.Refresh();

        }
    }

    private Gradient GenerateGradient(int keys, bool pb = false)
    {
        Gradient grad = new Gradient();
        GradientColorKey[] gradKeys = new GradientColorKey[keys];
        for(int i =0; i<keys; i++){
            float step = 1/(float)keys;
            if(pb){
                float value = UnityEngine.Random.Range(0.0f,1.1f);
                gradKeys[i] = new GradientColorKey(new Color(
                value,
                value,
                value),
                step*i);
            }else{
            gradKeys[i] = new GradientColorKey(new Color(
                UnityEngine.Random.Range(0.0f,1.1f),
                UnityEngine.Random.Range(0.0f,1.1f),
                UnityEngine.Random.Range(0.0f,1.1f)),
                step*i);
            }
        }
        grad.colorKeys = gradKeys;
        return grad;
    }

    public Texture2D GenerateGradTexture(Gradient grad, Gradient grad2, Gradient grad3){

        Texture2D tex = new Texture2D(textureWidth, textureHeight);
        for(int i =0; i<textureWidth; i++){
            for(int o = 0; o<textureHeight; o++){
                tex.SetPixel(i,o,
                Color.Lerp(grad.Evaluate(i/(float)textureWidth), 
                grad2.Evaluate(i/(float)textureWidth),o/(float)textureHeight)
                + grad3.Evaluate(o/(float)textureHeight) * intensity);
                
            }
        }
        tex.Apply();
        return tex;
    }
}