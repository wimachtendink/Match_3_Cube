using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayspaceLocation : MonoBehaviour
{
    public Vector3Int controllerPlayspacePostion;

    void Update()
    {
        controllerPlayspacePostion = Vector3Int.RoundToInt(transform.localPosition);
    }
}
