using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;

public class ControllerInputHandler : MonoBehaviour
{
    [Tooltip("Down is fired every time the trigger is pulled")]
    public UnityEvent OnRightControllerTriggerDown;
    [Tooltip("Hold is fired every frame after the trigger down even until the trigger up event")]
    public UnityEvent OnRightControllerTriggerHold;
    [Tooltip("Up is fired when the trigger was already down and is now up")]
    public UnityEvent OnRightControllerTriggerUp;

    [Tooltip("below is all the same but for left controller instead of right")]
    public UnityEvent OnLeftControllerTriggerDown;
    public UnityEvent OnLeftControllerTriggerHold;
    public UnityEvent OnLeftControllerTriggerUp;

    bool RightTriggerDown;
    bool LeftTriggerDown;
}
