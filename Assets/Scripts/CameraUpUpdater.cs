using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUpUpdater : MonoBehaviour
{
    public Transform CameraUpThing;
    public Material mat;

    bool Verified = false;

    // Start is called before the first frame update
    void Start()
    {
        if (mat.HasProperty("_Cam_Up"))
        {
            Verified = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Verified)
        {
            Vector3 v = CameraUpThing.position - Camera.current.transform.position;
            float[] vs = { v.x, v.y, v.z };
            mat.SetFloatArray("_Cam_Up", vs);
        }
        
    }
}
