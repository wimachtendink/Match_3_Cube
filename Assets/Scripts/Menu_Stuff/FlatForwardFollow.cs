using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FlatForwardFollow : MonoBehaviour
{

    public float DistanceToMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<XRNodeState> ns = new List<XRNodeState>();
        InputTracking.GetNodeStates(ns);

        
        Vector3 pos = Vector3.zero;
        Vector3 axis = Vector3.zero;
        Quaternion q = Quaternion.identity;
        float angle = 0.0f;

        foreach(var n in ns)
        {
            if (n.nodeType == XRNode.Head && n.TryGetPosition(out pos) && n.TryGetRotation(out q))
            {
                //I want to be @ position + quaternion.forward but quaternion.forward isn't a thing

                transform.position = pos;
                transform.rotation = q;

                transform.position = transform.position + (new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized * 0.5f);
                transform.rotation = Quaternion.LookRotation(pos - transform.position);
            }
        }
    }
}
