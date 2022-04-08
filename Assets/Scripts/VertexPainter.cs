using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPainter : MonoBehaviour
{

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetButton("Fire1")){ 
           var mouseposition = Input.mousePosition;
           mouseposition.z = cam.nearClipPlane;
            var worldPos = -cam.ScreenToWorldPoint(mouseposition);
            RaycastHit hit;
            Debug.Log(worldPos);
            Debug.DrawRay(cam.transform.position,worldPos*100);
            if(Physics.Raycast(cam.transform.position, worldPos, out hit)){
                //Debug.Log(hit.collider.name);
            }
       }
    }
}
