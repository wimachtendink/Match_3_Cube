using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Difficulty : MonoBehaviour
{
    public ButtonValue colors;
    public ButtonValue dimLen;
    public TextMeshProUGUI Display;

    public void UpdateDisplay()
    {
        Display.text = ((dimLen.value + colors.value) - 6) + "";
    }


}
