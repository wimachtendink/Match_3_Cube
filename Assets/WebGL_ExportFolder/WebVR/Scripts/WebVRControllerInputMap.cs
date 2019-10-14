using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WebVRControllerInputMap : MonoBehaviour
{
    [Header("Controller: left or right")]
    [SerializeField]
    public InputDeviceRole deviceRole;

    [Header("WebVR Controller Input Map")]
    public List<WebVRControllerInput> inputs;
}


