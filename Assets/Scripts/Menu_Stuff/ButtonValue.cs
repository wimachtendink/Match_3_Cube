using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonValue : MonoBehaviour
{
    public TextMeshProUGUI Value_Display;

    public int value;

    public int DefaultValue = 3;
    public int MaxValue;
    public int MinValue;

    public void Reset()
    {
        value = DefaultValue;
    }

    private void OnEnable()
    {
        value = DefaultValue;
        UpdateDisplay();
    }

    public void IncreaseValue()
    {
        if(value < MaxValue)
        {
            value++;
            UpdateDisplay();
        }
    }

    public void DecreaseValue()
    {
        if (value > MinValue)
        {
            value--;
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        Value_Display.text = value.ToString();
    }
}
