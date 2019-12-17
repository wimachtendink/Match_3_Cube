using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    public float radius;
    public UnityEvent EventToInvoke;

    //private void Start()
    //{
    //    ButtonManager.instance.menuButtons.Add(this);
    //}

    private void OnEnable()
    {
        ButtonManager.instance.menuButtons.Add(this);
    }

    private void OnDisable()
    {
        ButtonManager.instance.menuButtons.Remove(this);
    }
    public void OnClick()
    {
        EventToInvoke.Invoke();
    }
}
