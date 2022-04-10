using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseUpdater : MonoBehaviour
{
    [SerializeField] private int textureWidth,textureHeight,points;
    [SerializeField] private float moveSpeed,threashold;

    private Vector2[] pointsList, newPoints;
    [SerializeField]private Texture2D texture;
    [SerializeField]private Renderer rend;
    [SerializeField]private bool animate;

    void Start()
    {
        texture = Noises.GenerateVoronoiNoise(textureWidth, textureHeight, points);
        pointsList = Noises.GetVoronoiPoints();
        rend.material.SetTexture("_MainTex", texture);
    }

    // Update is called once per frame
    void Update()
    {
        if(animate){
            AnimatePointsPosition();
            texture = Noises.GenerateVoronoiNoise(textureWidth, textureHeight, points, pointsList);
            rend.material.SetTexture("_MainTex", texture);
        }
        if(Input.GetButton("Fire1")){
            UpdatePointPositionWithMouse(0);
        texture = Noises.GenerateVoronoiNoise(textureWidth, textureHeight, points, pointsList);
        rend.material.SetTexture("_MainTex", texture);

        }
    }

    private void AnimatePointsPosition(){
        if(newPoints == null){
            newPoints = new Vector2[pointsList.Length];
            for(int i =0; i<newPoints.Length; i++){
                newPoints[i] = new Vector2(UnityEngine.Random.Range(0.0f, 1.1f),UnityEngine.Random.Range(0.0f, 1.1f));
            }
        }
        for(int i = 0; i<pointsList.Length; i++){
            if(Vector2.Distance(pointsList[i], newPoints[i]) < threashold){
                newPoints[i] = new Vector2(UnityEngine.Random.Range(0.0f, 1.1f),UnityEngine.Random.Range(0.0f, 1.1f));
            }
            pointsList[i] = pointsList[i] + (newPoints[i] -pointsList[i]).normalized * moveSpeed * Time.deltaTime;
        }
    }
    private void UpdatePointPositionWithMouse(int v)
    {
        //pointsList[v].x += UnityEngine.Random.Range(-0.01f,0.01f);
        //pointsList[v].y += UnityEngine.Random.Range(-0.01f,0.01f);

        pointsList[v] = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
    }
}
