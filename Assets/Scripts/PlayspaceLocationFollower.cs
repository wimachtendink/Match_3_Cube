using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayspaceLocationFollower : MonoBehaviour
{

    public GameObject ThingToMove;
    public ControllerPlayspaceLocation cpl;

    void Update()
    {
        ThingToMove.transform.localPosition = cpl.controllerPlayspacePostion;
    }
}
