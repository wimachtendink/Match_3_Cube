using UnityEngine;
using UnityEngine.XR;

public class ControllerInteraction : MonoBehaviour
{
    WebVRController controller;
#if UNITY_EDITOR
    private void Start()
    {
        controller = gameObject.GetComponent<WebVRController>();
    }

    void Update()
    {
        if (XRDevice.isPresent)
        {
            foreach (WebVRControllerInput input in controller.inputMap.inputs)
            {
                bool state = XRInputMaster.ButtonCheck(controller.inputMap.deviceRole, input.usageFeature);
                if (input.PreviousState != state)
                {
                    if (state)
                    {
                        input.PreviousState = state;
                        input.OnDown.Invoke();
                    }
                    else
                    {
                        input.PreviousState = state;
                        input.OnUp.Invoke();
                    }
                }
            }
        }
    }
#endif
}