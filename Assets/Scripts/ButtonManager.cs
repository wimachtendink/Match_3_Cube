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

    public void CheckForMenuButtonClick()
    {
        CheckForMenuButtonClick(SelectController.position);
    }

    public void CheckForMenuButtonClick(Vector3 _pos)
    {
        print("checking for buttons pressed!");
        foreach(var m in menuButtons)
        {
            if(Vector3.Distance(m.transform.position, _pos) < m.radius)
            {
                m.OnClick();
            }
        }
    }

}
