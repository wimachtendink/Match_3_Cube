using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum WebVRControllerHand { NONE, LEFT, RIGHT };
[System.Serializable]
public class WebVRControllerButton
{
    //it might be good to keep XR role here also, but might just be bloat
    public bool pressed;
    public bool prevPressedState;
    public bool touched;
    public float value;

    public WebVRControllerButton(bool isPressed, float buttonValue)
    {
        pressed = isPressed;
        prevPressedState = false;
        value = buttonValue;
    }
}

public class WebVRController : MonoBehaviour
{
    [Tooltip("Controller hand to use.")]
    public WebVRControllerHand hand = WebVRControllerHand.NONE;
    [Tooltip("Controller input settings.")]
    public WebVRControllerInputMap inputMap;
    [Tooltip("Simulate 3dof controller")]
    public bool simulate3dof = false;
    [Tooltip("Vector from head to elbow")]
    public Vector3 eyesToElbow = new Vector3(0.1f, -0.4f, 0.15f);
    [Tooltip("Vector from elbow to hand")]
    public Vector3 elbowHand = new Vector3(0, 0, 0.25f);
    
    
    private Matrix4x4 sitStand;
    private float[] axes;

    private XRNode handNode;
    private Quaternion headRotation;
    private Vector3 headPosition;
    public Dictionary<string, WebVRControllerButton> buttonStates = new Dictionary<string, WebVRControllerButton>();

#if UNITY_EDITOR

    private void Awake()
    {
        print("using an awake, probably need to double sheck that this is needed in WebGL build");

        if(hand == WebVRControllerHand.LEFT)
        {
            handNode = XRNode.LeftHand;
        }else if(hand == WebVRControllerHand.RIGHT)
        {
            handNode = XRNode.RightHand;
        }

    }


#endif

    // Updates button states from Web gamepad API.
    //instead we should just fire our buttons, right?
    private void UpdateButtons(WebVRControllerButton[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            WebVRControllerButton button = buttons[i];
            foreach (WebVRControllerInput input in inputMap.inputs)
            {
                if (input.gamepadId == i)
                {
                    //SetButtonState(input.actionName, button.pressed, button.value);
                    //why not just do stuff here?
                    if(input.PreviousState != button.pressed)
                    {
                        //something has changed!
                        if(button.pressed)
                        {
                            //this means we were NOT PRESSED and now we are!
                            //this is DOWN
                            input.PreviousState = button.pressed;
                            input.OnDown.Invoke();
                        }
                        else
                        {
                            //this means we were PRESSED now we are NOT
                            //this is UP
                            input.PreviousState = button.pressed;
                            input.OnUp.Invoke();
                        }
                    }
                }
            }
        }
    }

    public bool GetButton(string action)
    {
        if (XRDevice.isPresent)
        {
            foreach(WebVRControllerInput input in inputMap.inputs)
            {
                if (action == input.actionName)// && input.unityInputIsButton)
                {
                    return XRInputMaster.ButtonCheck(inputMap.deviceRole, input.usageFeature);
                }
            }
        }

        if (!buttonStates.ContainsKey(action))
            return false;
        return buttonStates[action].pressed;
    }

    private bool GetPastButtonState(string action)
    {
        if (!buttonStates.ContainsKey(action))
            return false;
        return buttonStates[action].prevPressedState;
    }

    //making dynamic button list, but I kinda already have this?
    private void SetButtonState(string action, bool isPressed, float value)
    {
        if (buttonStates.ContainsKey(action))
        {
            buttonStates[action].pressed = isPressed;
            buttonStates[action].value = value;
        }
        else
            buttonStates.Add(action, new WebVRControllerButton(isPressed, value));
    }

    private void SetPastButtonState(string action, bool isPressed)
    {
        if (!buttonStates.ContainsKey(action))
            return;
        buttonStates[action].prevPressedState = isPressed;
    }

    public bool GetButtonDown(string action)
    {
        // Use Unity Input Manager when XR is enabled and WebVR is not being used (eg: standalone or from within editor).
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            foreach(WebVRControllerInput input in inputMap.inputs)
            {
                if (action == input.actionName) // && input.unityInputIsButton)
                {
                    return XRInputMaster.ButtonCheck(inputMap.deviceRole, input.usageFeature);
                    //return Input.GetButtonDown(input.unityInputName);
                }
            }
        }

        if (GetButton(action) && !GetPastButtonState(action))
        {
            SetPastButtonState(action, true);
            return true;
        }
        return false;
    }

    //we can only get up if we were already down
    public bool GetButtonUp(string action)
    {

        // Use Unity Input Manager when XR is enabled and WebVR is not being used (eg: standalone or from within editor).
        if (XRDevice.isPresent)
        {
            foreach(WebVRControllerInput input in inputMap.inputs)
            {
                //true if down
                if (action == input.actionName && input.PreviousState) // && input.unityInputIsButton)
                {
                    return XRInputMaster.ButtonCheck(inputMap.deviceRole, input.usageFeature);
                    //return Input.GetButtonUp(input.unityInputName);
                }
            }
        }

        if (!GetButton(action) && GetPastButtonState(action))
        {
            SetPastButtonState(action, false);
            return true;
        }
        return false;
    }

    private void onHeadsetUpdate(Matrix4x4 leftProjectionMatrix,
        Matrix4x4 rightProjectionMatrix,
        Matrix4x4 leftViewMatrix,
        Matrix4x4 rightViewMatrix,
        Matrix4x4 sitStandMatrix)
    {
        Matrix4x4 trs = WebVRMatrixUtil.TransformViewMatrixToTRS(leftViewMatrix);
        this.headRotation = WebVRMatrixUtil.GetRotationFromMatrix(trs);
        this.headPosition = WebVRMatrixUtil.GetTranslationFromMatrix(trs);
        this.sitStand = sitStandMatrix;
    }

    private void onControllerUpdate(string id,
        int index,
        string handValue,
        bool hasOrientation,
        bool hasPosition,
        Quaternion orientation,
        Vector3 position,
        Vector3 linearAcceleration,
        Vector3 linearVelocity,
        WebVRControllerButton[] buttonValues,
        float[] axesValues)
    {
        if (handFromString(handValue) == hand)
        {
            SetVisible(true);

            Quaternion sitStandRotation = WebVRMatrixUtil.GetRotationFromMatrix(this.sitStand);
            Quaternion rotation = sitStandRotation * orientation;

            if (!hasPosition || this.simulate3dof) {
                position = applyArmModel(
                    this.sitStand.MultiplyPoint(this.headPosition),
                    rotation,
                    this.headRotation);
            }
            else
            {
                position = this.sitStand.MultiplyPoint(position);
            }

            transform.rotation = rotation;
            transform.position = position;

            UpdateButtons(buttonValues);
            this.axes = axesValues;
        }
    }

    private WebVRControllerHand handFromString(string handValue)
    {
        WebVRControllerHand handParsed = WebVRControllerHand.NONE;

        if (!String.IsNullOrEmpty(handValue)) {
            try
            {
                handParsed = (WebVRControllerHand) Enum.Parse(typeof(WebVRControllerHand), handValue.ToUpper(), true);
            }
            catch
            {
                Debug.LogError("Unrecognized controller Hand '" + handValue + "'!");
            }
        }
        return handParsed;
    }

    private void SetVisible(bool visible) {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
        {
            foreach (Renderer component in rendererComponents) {
                component.enabled = visible;
            }
        }
    }

    // Arm model adapted from: https://github.com/aframevr/aframe/blob/master/src/components/tracked-controls.js
    private Vector3 applyArmModel(Vector3 controllerPosition, Quaternion controllerRotation, Quaternion headRotation)
    {
        // Set offset for degenerate "arm model" to elbow.
        Vector3 deltaControllerPosition = new Vector3(
            this.eyesToElbow.x * (this.hand == WebVRControllerHand.LEFT ? -1 : this.hand == WebVRControllerHand.RIGHT ? 1 : 0),
            this.eyesToElbow.y,
            this.eyesToElbow.z);

        // Apply camera Y rotation (not X or Z, so you can look down at your hand).
        Quaternion headYRotation = Quaternion.Euler(0, headRotation.eulerAngles.y, 0);
        deltaControllerPosition = (headYRotation * deltaControllerPosition);
        controllerPosition += deltaControllerPosition;

        // Set offset for forearm sticking out from elbow.
        deltaControllerPosition.Set(this.elbowHand.x, this.elbowHand.y, this.elbowHand.z);
        deltaControllerPosition = Quaternion.Euler(controllerRotation.eulerAngles.x, controllerRotation.eulerAngles.y, 0) * deltaControllerPosition;
        controllerPosition += deltaControllerPosition;
        return controllerPosition;
    }

    void Update()
    {
        // Use Unity XR Input when enabled. When using WebVR, updates are performed onControllerUpdate.
        //I need to rewrite this to take XRDevice and get input which corresponds to the thing
        if (XRDevice.isPresent)
        {
            List<XRNodeState> states = new List<XRNodeState>();
            InputTracking.GetNodeStates(states);

            Vector3 newPos = Vector3.zero;
            Quaternion newRot = Quaternion.identity;
            foreach(var state in states)
            {
                if(state.nodeType == handNode && state.TryGetPosition(out newPos) && state.TryGetRotation(out newRot))
                {
                    transform.position = newPos;
                    transform.rotation = newRot;
                }
            }
            SetVisible(true);
        }
    }

    void OnEnable()
    {
        if (inputMap == null)
        {
            Debug.LogError("A Input Map must be assigned to WebVRController!");
            return;
        }
        WebVRManager.Instance.OnControllerUpdate += onControllerUpdate;
        WebVRManager.Instance.OnHeadsetUpdate += onHeadsetUpdate;
        SetVisible(false);
    }

    void OnDisabled()
    {
        WebVRManager.Instance.OnControllerUpdate -= onControllerUpdate;
        WebVRManager.Instance.OnHeadsetUpdate -= onHeadsetUpdate;
        SetVisible(false);
    }
}
