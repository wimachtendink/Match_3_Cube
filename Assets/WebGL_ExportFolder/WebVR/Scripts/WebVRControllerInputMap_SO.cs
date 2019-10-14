using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;



[CreateAssetMenu(menuName = "WebVRControllerInputMap")]
public class WebVRControllerInputMap_SO : ScriptableObject
{
    [Header("Controller: left or right")]
    [SerializeField]
    public InputDeviceRole deviceRole;

    [Header("WebVR Controller Input Map")]
    public List<WebVRControllerInput> inputs;
}



[System.Serializable]
public class WebVRControllerInput {
    [Tooltip("A meaningful name describing the gesture performed on the controller.")]
    public string actionName;

    [Header("Web Gamepad API configuration")]
    [Tooltip("Button or axes ID from Web Gamepad API.")]
    public int gamepadId;
    [Tooltip("Whether gesture derives its value from a Gamepad API `GamepadButton.pressed` property.")]
    public bool gamepadIsButton;

    [Header("Unity XR INPUT configuration")]
    //[Tooltip("Input name defined in Unity Input Manager.")]
    //public string unityInputName;
    //[Tooltip("Whether gesture derives its value from Unity using `Input.GetButton` function.")]
    //public bool unityInputIsButton;

    public XRInputMaster.CommonFeaturesString usageFeature;

    public bool PreviousState = false;

    public UnityEvent OnDown;
    public UnityEvent OnUp;

}
