using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class GradientGenerator : EditorWindow
{    
    public enum GradientType {Single, DoubleNoised, DoubleVertical, DoubleVerticalInterPolated};
    public GradientType currentGradient;

    int textureWidth, textureHeight;
    string textureName;
    Gradient verticalGradient = new Gradient();
    Gradient verticalGradient2 = new Gradient();

    public int keys { get; private set; }
    public int keys2 { get; private set; }
    public int keys3 { get; private set; }

    Gradient horizontalGradient = new Gradient();
    float verticalIntensity = 0.5f;
    float horizontalIntensity = 0.5f;

    bool autoGenerate = false;
    float noiseScale = 5;
    Vector2 noiseOffset = new Vector2(0,0);
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

        currentGradient = (GradientType)EditorGUILayout.EnumPopup(currentGradient);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Texture Width");
        textureWidth = EditorGUILayout.IntField(textureWidth);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Texture Width");
        textureHeight = EditorGUILayout.IntField(textureHeight);
        GUILayout.EndHorizontal();

        switch(currentGradient){
            case GradientType.Single : 
                SingleGradientWindow();
            break;
            case GradientType.DoubleNoised : 
                DoubleGradientWindow();
            break;
            case GradientType.DoubleVertical : 
                DoubleVerticalGradientWindow();
            break;

            case GradientType.DoubleVerticalInterPolated : 
                DoubleInterpolatedGradientWindow();
            break;
        }
        autoGenerate = EditorGUILayout.Toggle(autoGenerate);
        if(autoGenerate){
            generatedTexture = GenerateGradTexture();

        }
        else{ 
            if(GUILayout.Button("Generate Texture")) {
                generatedTexture = GenerateGradTexture();
            }
        }
        GUILayout.Box(generatedTexture);
        textureName = EditorGUILayout.TextField(textureName);
        if(GUILayout.Button("Create Texture")){
            CreateTexture();
        }
    }

    private void CreateTexture()
    {
        byte[] bytes = generatedTexture.EncodeToPNG();
        if(!Directory.Exists(Application.dataPath + "/ExportedGradient/")){
            Directory.CreateDirectory(Application.dataPath + "/ExportedGradient/");
        }
        File.WriteAllBytes(Application.dataPath + "/ExportedGradient/" + textureName+ ".png", bytes);
        AssetDatabase.Refresh();
    }


///Gradient windows

    private void SingleGradientWindow()
    {
        GUILayout.BeginHorizontal();
        verticalGradient = EditorGUILayout.GradientField("Gradient", verticalGradient);
        keys = EditorGUILayout.IntSlider(keys,2,8);

        if(GUILayout.Button("Random Gradient")){
            verticalGradient = GenerateGradient(keys);
        }
        GUILayout.EndHorizontal();
    }

    private void DoubleGradientWindow()
    {
                GUILayout.BeginHorizontal();
        verticalGradient = EditorGUILayout.GradientField("Gradient", verticalGradient);
        keys = EditorGUILayout.IntSlider(keys,2,8);

        if(GUILayout.Button("Random Gradient")){
            verticalGradient = GenerateGradient(keys);
        }
        GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
        verticalGradient2 = EditorGUILayout.GradientField("Gradient", verticalGradient2);
        keys2 = EditorGUILayout.IntSlider(keys2,2,8);

        if(GUILayout.Button("Random Gradient")){
            verticalGradient2 = GenerateGradient(keys2);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Noise Scale");
        noiseScale = EditorGUILayout.FloatField(noiseScale);
        GUILayout.EndHorizontal();
        
        noiseOffset = EditorGUILayout.Vector2Field("Noise Position Offset", noiseOffset);

    }

    private void DoubleVerticalGradientWindow()
    {
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
    }


    private void DoubleInterpolatedGradientWindow()
    {
        GUILayout.BeginHorizontal();
        verticalGradient = EditorGUILayout.GradientField("Gradient", verticalGradient);
        keys = EditorGUILayout.IntSlider(keys,2,8);

        if(GUILayout.Button("Random Gradient")){
            verticalGradient = GenerateGradient(keys);
        }
        GUILayout.EndHorizontal();
         GUILayout.BeginHorizontal();
        verticalGradient2 = EditorGUILayout.GradientField("Gradient", verticalGradient2);
        keys2 = EditorGUILayout.IntSlider(keys2,2,8);

        if(GUILayout.Button("Random Gradient")){
            verticalGradient2 = GenerateGradient(keys2);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        horizontalGradient = EditorGUILayout.GradientField("Gradient", horizontalGradient);
        keys3 = EditorGUILayout.IntSlider(keys3,2,8);

        if(GUILayout.Button("Random Gradient")){
            horizontalGradient = GenerateGradient(keys3, true);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Vertical Interpolation with Third Gradient");
        verticalIntensity = EditorGUILayout.FloatField(verticalIntensity);    
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Horizontal Interpolation with Third Gradient");
        horizontalIntensity = EditorGUILayout.FloatField(horizontalIntensity);    
        GUILayout.EndHorizontal();
    }


   
///Gradient Generation
    private Gradient GenerateGradient(int keys, bool pb = false)
    {
        Gradient grad = new Gradient();
        GradientColorKey[] gradKeys = new GradientColorKey[keys];
        for(int i =0; i<keys; i++){
            float step = 1/(float)(keys-1);
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


    public Texture2D GenerateGradTexture(){

        switch(currentGradient){
            case GradientType.Single:
                return GenerateSingleGradient(verticalGradient);
            case GradientType.DoubleNoised:
                return GenerateDoubleGradient(verticalGradient, verticalGradient2);
            case GradientType.DoubleVertical:
               return GenerateDoubleVerticalGradient(verticalGradient, verticalGradient2);
            case GradientType.DoubleVerticalInterPolated:
                return GenerateDoubleVerticalInterpolatedGrad(verticalGradient, verticalGradient2, horizontalGradient);
        }
        return null;
    }


///Gradient Generation Algorithms
private Texture2D GenerateSingleGradient(Gradient grad)
    {
        Texture2D tex = new Texture2D(textureWidth, textureHeight);
        for(int i =0; i<textureWidth; i++){
            for(int o = 0; o<textureHeight; o++){
                tex.SetPixel(i,o,grad.Evaluate(i/(float)textureWidth));
                
            }
        }
        tex.Apply();
        return tex;    
    }
    private Texture2D GenerateDoubleGradient(Gradient grad, Gradient grad2)
    {
        Texture2D tex = new Texture2D(textureWidth, textureHeight);
        for(int i =0; i<textureWidth; i++){
            for(int o = 0; o<textureHeight; o++){
               float xCoord = ((float)i + noiseOffset.x) / textureWidth * noiseScale;
                float yCoord = ((float)o + noiseOffset.y)/ textureHeight * noiseScale;
                float n = Mathf.PerlinNoise(xCoord, yCoord);
                tex.SetPixel(i,o,
               Color.Lerp( 
                grad2.Evaluate(o/(float)textureWidth),
                grad.Evaluate(i/(float)textureWidth),
                n));
            }
        }
        tex.Apply();
        return tex;
    }

    private Texture2D GenerateDoubleVerticalGradient(Gradient grad, Gradient grad2)
    {
         Texture2D tex = new Texture2D(textureWidth, textureHeight);
        for(int i =0; i<textureWidth; i++){
            for(int o = 0; o<textureHeight; o++){
                tex.SetPixel(i,o,
                (grad.Evaluate(i/(float)textureWidth) + 
                grad2.Evaluate(i/(float)textureWidth))/2);
            }
        }
        tex.Apply();
        return tex;
    }

    public Texture2D GenerateDoubleVerticalInterpolatedGrad(Gradient grad, Gradient grad2, Gradient grad3){

        Texture2D tex = new Texture2D(textureWidth, textureHeight);
        for(int i =0; i<textureWidth; i++){
            for(int o = 0; o<textureHeight; o++){
                tex.SetPixel(i,o,
                Color.Lerp(grad.Evaluate(i/(float)textureWidth), 
                grad2.Evaluate(i/(float)textureWidth),
                (grad3.Evaluate((i/(float)textureWidth)).r * verticalIntensity+
                 grad3.Evaluate((o/(float)textureHeight)).r* horizontalIntensity)  /2));
                
            }
        }
        tex.Apply();
        return tex;
    }
}