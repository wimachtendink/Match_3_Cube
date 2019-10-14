using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{

    public Transform thingToMove;
    public Transform thingToFollow;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Update()
    {
        thingToMove.position = thingToFollow.position;
        thingToMove.rotation = thingToFollow.rotation;
    }

}
