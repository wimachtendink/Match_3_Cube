using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitwiseTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float start = Time.realtimeSinceStartup;
    }


    public void CheckABunch()
    {
        
        int a = Random.Range(int.MinValue, int.MaxValue);

        int b = a;

        print(a & 7);

        int output = 0;
        
        int i = 0;
        for (; a > 0; i++)
        {
            output <<= 1;
            output += a & 1;
            a >>= 1;
        }

        a = 0;
        for (; i > 0; i--)
        {
            a <<= 1;
            a += output & 1;
            output >>= 1;
        }

        if (b == a)
        {
            //print("wow, I can't believe that worked! a: " + a + " b: " + b + " output: " + output);
        }
        else
        {
            //print("nope, that didn't work, a: " + a + " b: " + b + " output: " + output);
        }
    }
}
