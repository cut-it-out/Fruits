using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static float CamWidth = 0f;
    
    // Start is called before the first frame update
    void Awake()
    {
        Camera cam = Camera.main;
        CamWidth =  cam.orthographicSize * cam.aspect;
        cam.transform.position = new Vector3(CamWidth, cam.transform.position.y, cam.transform.position.z);
    }

}
