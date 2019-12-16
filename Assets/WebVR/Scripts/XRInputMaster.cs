using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;

//I need to split this into in and out so I can arbitrarily assign things to trigger controller events?
public class XRInputMaster : MonoBehaviour
{
    public enum CommonFeaturesString
    {
        primary2DAxisClick,
        trigger,
        grip
    }

    public InputDeviceRole inputDeviceRole;

    public CommonFeaturesString inputType;

    [Serializable]
    public class XRControllerEvent_Vec2 : UnityEvent<Vector2> { }

    public InputFeatureUsage<bool> InputFeatureUsage;

    List<UnityEvent> lefts;
    bool l_trigger_down;
    //LeftController
    //Trigger
    [Header("\t Trigger")]

    [Header("Left Controller")]
    public UnityEvent OnLeftTriggerDown = new UnityEvent();
    public UnityEvent OnLeftTriggerUp = new UnityEvent();
    //padClick
    [Header("\t Pad")]
    bool l_pad_down;
    public UnityEvent OnLeftPadDown = new UnityEvent();
    public UnityEvent OnLeftPadUp = new UnityEvent();
    Vector2 l_axis2;
    public XRControllerEvent_Vec2 OnLeftPadLoc = new XRControllerEvent_Vec2();

    //Right controller
    //Trigger
    List<UnityEvent> rights;
    bool r_trigger_down;

    [Header("\t Trigger")]

    [Header("Right Controller")]
    [Header("---")]
    public UnityEvent OnRightTriggerDown = new UnityEvent();
    public UnityEvent OnRightTriggerUp = new UnityEvent();

    //padClick
    [Header("\t Pad")]
    bool r_pad_down;
    public UnityEvent OnRightPadDown = new UnityEvent();
    public UnityEvent OnRightPadUp = new UnityEvent();
    Vector2 r_axis2;
    public XRControllerEvent_Vec2 OnRightPadLoc = new XRControllerEvent_Vec2();


    //  -   -   -   STATIC METHODS  -   -   -   //

    public static InputFeatureUsage<bool> GetFeature(CommonFeaturesString s)
    {
        if (s == CommonFeaturesString.trigger)
        {
            return CommonUsages.triggerButton;
        }

        if (s == CommonFeaturesString.primary2DAxisClick)
        {
            return CommonUsages.primary2DAxisClick;
        }

        if (s == CommonFeaturesString.grip)
        {
            return CommonUsages.gripButton;
        }
        return new InputFeatureUsage<bool>();//not totally sure what will happen here, but I'm fairly certain that there's no way that a controller will have this arbitrary feature usage...
    }

    //probably an opportinuty for imporvement, since I'm getting the controller and checking if it exists for every button
    public static bool ButtonCheck(InputDeviceRole role, CommonFeaturesString inputFeatureUsage)
    {
        var device = new List<InputDevice>();

        InputDevices.GetDevicesWithRole(role, device);

        if(device.Count > 0)
        {
            if (device[0].TryGetFeatureValue(GetFeature(inputFeatureUsage), out bool controlValue))
            {

                return controlValue;
            }
            else
            {
                Debug.LogError("Match3_Cube: attempted to get a button which is not acessible on this controller!");
                return false;
            }
        }
        return false;
    }
    //  -   -   -   MEMBER METHODS  -   -   -   //

    //public void Start()
    //{
    //    l_trigger_down = false;
    //    l_pad_down = false;
    //    r_trigger_down = false;
    //    r_pad_down = false;
    //    Debug.Log("InputMaster started fine");
    //}





    // Update is called once per frame
//    void Update()
//    {
//        var LeftController = new List<InputDevice>();
//        var RightController = new List<InputDevice>();


//        InputDevices.GetDevicesWithRole(InputDeviceRole.LeftHanded, LeftController);
//        InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, RightController);

//        if (LeftController.Count > 0)
//        {
//            HandleLeft(LeftController[0]);
//        }

//        if (RightController.Count > 0)
//        {
//            HandleRight(RightController[0]);
//        }

//        //var deviceList = new List<InputDevice>();
//        //InputDevices.GetDevices(deviceList);

////        foreach (InputDevice device in deviceList)
////        {
////            //which controller
////            //which button
////            //bool primary2dClick;
////            if (device.name.ToLower().Contains("controller")) // && device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primary2dClick) && primary2dClick)
////            {
//////                print("we found a controller");
////                //do controllerStuff!
////                if (device.name.ToLower().Contains("left"))
////                {
////                    //print("left is the controller");
////                    HandleLeft(device);
////                }
////                else// if(device.name.ToLower().Contains("left"))
////                {
////                    HandleRight(device);
////                    //print("right is the controller");
////                }
////            }

////        }
//    }

    //could be omproved possibly by making Left and Right lists, then just HandleController with lists
//    public void HandleLeft(InputDevice d)
//    {
//        //trigger
//        bool trigger;
//        if(d.TryGetFeatureValue(CommonUsages.triggerButton, out trigger))
//        {
//            //is it better to first check if they match, then figure out which case, or just check the cases?
//                //if change
//                    //if down -> op
//                    //if up -> down
//            if(trigger != l_trigger_down)
//            {
//                if(l_trigger_down)
//                {
//                    OnLeftTriggerUp.Invoke();
//                    l_trigger_down = !l_trigger_down;
//                }
//                else
//                {
//                    OnLeftTriggerDown.Invoke();
//                    l_trigger_down = !l_trigger_down;
//                }
//            } 
//        }
//        //pad
//        bool pad;
//        if (d.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out pad))
//        {
//            //is it better to first check if they match, then figure out which case, or just check the cases?
//            //if change
//            //if down -> op
//            //if up -> down
//            if (pad != l_pad_down)
//            {
//                if (l_pad_down)
//                {
//                    OnLeftPadUp.Invoke();
//                    l_pad_down = !l_pad_down;
//                }
//                else
//                {
//                    OnLeftPadDown.Invoke();
//                    l_pad_down = !l_pad_down;
//                }
//            }
//        }
//    }
//    public void HandleRight(InputDevice d)
//    {
//        //trigger
//        bool trigger;
//        if (d.TryGetFeatureValue(CommonUsages.triggerButton, out trigger))
//        {
//            //is it better to first check if they match, then figure out which case, or just check the cases?
//            //if change
//            //if down -> up
//            //if up -> down
//            if (trigger != r_trigger_down)
//            {
//                if (r_trigger_down)//this means it was already down but is now up
//                {
//                    OnRightTriggerUp.Invoke();
//                    r_trigger_down = false;
//                }
//                else//  this means it was already up and is now down?
//                {

////                    print("firing trigger!");
//                    OnRightTriggerDown.Invoke();
//                    r_trigger_down = true;
//                }
//            }
//        }
//        //pad
//        bool pad;
//        if (d.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out pad))
//        {
//            //is it better to first check if they match, then figure out which case, or just check the cases?
//            //if change
//            //if down -> op
//            //if up -> down
//            if (pad != r_pad_down)
//            {
//                if (r_pad_down)
//                {
//                    OnRightPadUp.Invoke();
//                    r_pad_down = !r_pad_down;
//                }
//                else
//                {
//                    OnRightPadDown.Invoke();
//                    r_pad_down = !r_pad_down;
//                }
//            }
//        }
//    }
}
