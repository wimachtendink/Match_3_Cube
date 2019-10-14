using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAndMove : MonoBehaviour
{
    public Transform Controller;
    public Transform ScaleAndMoveMe;

    private Vector3 startPosition;

    [Tooltip("states")]
    [SerializeField]
    private bool moving;
    [SerializeField]
    private bool scaling;

    public float MinimumScale;
    [RangeAttribute(0.9f, 1.5f)]
    public float scaleSpeed;

    private float currentScale;
    private float previousScalerHeight;

    private void Start()
    {
        if (MinimumScale == 0)
        {
            MinimumScale = 0.2f;
        }
        currentScale = 1;
    }

    public void Reset()
    {
        ScaleAndMoveMe.position = Vector3.zero;

        ScaleAndMoveMe.localScale = Vector3.one;
        currentScale = 1;
    }

    public void Moving_Start()
    {
        moving = true;
        startPosition = Controller.position;
    }

    //make this a coroutine while down is true?
    public void Moving_Update()
    {
        ScaleAndMoveMe.position -= startPosition - Controller.position;
        startPosition = Controller.position;
    }

    public void Update()
    {
        if (moving)
        {
            Moving_Update();
        }

        if (scaling)
        {
            Scaling_Update();
        }
    }

    public void Moving_End()
    {
        moving = false;
        startPosition = Controller.position;
    }
    
    public void Scaling_Start()
    {
        previousScalerHeight = Controller.position.y;
        scaling = true;

    }

    public void Scaling_End()
    {
        scaling = false;
    }

    public void Scaling_Update()
    {
        ChangeScaleBy(Controller.position.y - previousScalerHeight);
        previousScalerHeight = Controller.position.y;
    }

    private void ChangeScaleBy(float AmountByWhichToChangeScale)
    {
        if (AmountByWhichToChangeScale + currentScale > MinimumScale)
        {
            currentScale += AmountByWhichToChangeScale * (currentScale * scaleSpeed); //the bigger currentScale is, the larger I would like this change to become...
            ScaleAndMoveMe.localScale = Vector3.one * currentScale;
        }
    }
}
