using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU_lookAt : MonoBehaviour
{
    public Transform thingToLookAt;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(thingToLookAt);
    }
}
