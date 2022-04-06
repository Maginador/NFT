using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnim : MonoBehaviour
{
    public Material mat;
    public Vector2 offset; 

    public float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mat)mat.mainTextureOffset += offset * speed * Time.deltaTime;
    }
}
