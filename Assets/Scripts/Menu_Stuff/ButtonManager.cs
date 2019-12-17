using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;

    public Transform SelectController;

    public List<MenuButton> menuButtons;

    private void Awake()
    {
        instance = this;
        menuButtons = new List<MenuButton>();
    }

    public bool CheckForMenuButtonClick()
    {
        return CheckForMenuButtonClick(SelectController.position);
    }

    public bool CheckForMenuButtonClick(Vector3 _pos)
    {
        Debug.Log("checking buttons for presses");
        foreach (var m in menuButtons)
        {
            if (Vector3.Distance(m.transform.position, _pos) < m.radius)
            {
                m.OnClick();
                return true;
            }
        }

        return false;
    }

}
