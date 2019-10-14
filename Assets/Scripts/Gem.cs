using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum dir
{
    up,
    down,
    right,
    left,
    forward,
    backward
}

public enum GemType
{

    Vivid_Yellow,
    Strong_Purple,
    Vivid_Orange,
    Very_Light_Blue,
    Vivid_Red,
    Grayish_Yellow,
    Medium_Gray,


    none
}

public class Gem : MonoBehaviour
{

    int mostRecentGemMax;

    //  --  --  --  --  --  --  --  --  class stuff

    public static List<Vector3Int> directions = new List<Vector3Int>
    {
        new Vector3Int( 1,    0,   0),
        new Vector3Int(-1,    0,   0),
        new Vector3Int( 0,    1,   0),
        new Vector3Int( 0,   -1,   0),
        new Vector3Int( 0,    0,   1),
        new Vector3Int( 0,    0,  -1),
    };

    //TODO: Consider removing GemType, it might not be necessary
    public static Dictionary<GemType, Color> GemColors = new Dictionary<GemType, Color>
    {
        {GemType.Vivid_Yellow,       ColorList._kellysMaxContrastSet[0]},
        {GemType.Strong_Purple,      ColorList._kellysMaxContrastSet[1]},
        {GemType.Vivid_Orange,       ColorList._kellysMaxContrastSet[2]},
        {GemType.Very_Light_Blue,    ColorList._kellysMaxContrastSet[3]},
        {GemType.Vivid_Red,          ColorList._kellysMaxContrastSet[4]},
        {GemType.Grayish_Yellow,     ColorList._kellysMaxContrastSet[5]},
        {GemType.Medium_Gray,        ColorList._kellysMaxContrastSet[6]},
    };

    public static IEnumerator AnimateDoubleSwap(Gem g1, Gem g2, bool reverse)
    {
        float maxDuration = 0.5f;
        float duration = 0.0f;

        float t = 0;

        Vector3 axis = Vector3.right;
        if (g1.Address - g2.Address == Vector3Int.right || g1.Address - g2.Address == Vector3Int.left)
        {
            axis = Vector3.up;
        }

        Vector3Int g1Start = g1.Address;
        Vector3Int g1End = g2.Address;
        if (reverse)
        {
            g1Start = g2.Address;
            g1End = g1.Address;
        }

        while (duration < maxDuration && duration > -0.001f)
        {
            t = duration / maxDuration;
            g1.transform.localPosition = Vector3.Lerp(g1Start, g1End, t) + axis * (0.5f - Mathf.Abs(0.5f - t));
            g2.transform.localPosition = Vector3.Lerp(g1End, g1Start, t) + -axis * (0.5f - Mathf.Abs(0.5f - t));

            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    //  --  --  --  --  --  --  --  --  Member stuff

    public Vector3Int Address;
    public GemType myType;

    public void MakeNewGem()
    {
        //I need a way to make this adjust with colors list
        myType = (GemType)Random.Range(0, mostRecentGemMax);
        name = Address + myType.ToString() ;
    }

    public void MakeNewRandomGem(int max)
    {
        mostRecentGemMax = max;
        myType = (GemType)Random.Range(0, max);
        name = Address + myType.ToString();
    }


    public void MakeNewGemOfType(GemType myNewType)
    {
        myType = myNewType;
        name = Address + myType.ToString();
    }


    public void AnimateAppear()
    {
        MeshRenderer m = gameObject.GetComponent<MeshRenderer>();
        MaterialPropertyBlock p = new MaterialPropertyBlock();
        ChangeGemTypeAppearance(m, p);
        StartCoroutine(AnimateGrow(m, p));
    }

    public IEnumerator UpdateGemAppearance_Coroutine(bool animate)
    {
        MeshRenderer m = gameObject.GetComponent<MeshRenderer>();
        MaterialPropertyBlock p = new MaterialPropertyBlock();

        if (animate)
        {
            yield return StartCoroutine(AnimateShrink(m, p));
            MakeNewGem();
            ChangeGemTypeAppearance(m, p);
            yield return StartCoroutine(AnimateGrow(m, p));
        }
        else
        {
            UpdateShape(p);
            UpdateColor(p);
            m.SetPropertyBlock(p);
        }
    }


    public IEnumerator AnimateReAppear()
    {
        MeshRenderer m = gameObject.GetComponent<MeshRenderer>();
        MaterialPropertyBlock p = new MaterialPropertyBlock();

        MakeNewGem();
        ChangeGemTypeAppearance(m, p);
        yield return StartCoroutine(AnimateGrow(m, p));
    }

    public IEnumerator AnimateDrop()
    {

        float maxDuration = 1.0f;
        float duration = 0.0f;

        Vector3 start = transform.localPosition;

        while (duration < maxDuration)
        {

            transform.localPosition = Vector3.Lerp(start, Address, Mathf.Pow(duration / maxDuration, 2));

            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = Address;
    }


    public IEnumerator AnimateDeal(Vector3Int start)
    {

        float maxDuration = 0.5f;
        float duration = 0.0f;
        
        while (duration < maxDuration)
        {

            transform.localPosition = Vector3.Lerp(start, Address, Mathf.Pow(duration/maxDuration, 2));

            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = Address;
    }

    public IEnumerator MakeGemDissapear(bool animate)
    {
        MeshRenderer m = gameObject.GetComponent<MeshRenderer>();
        MaterialPropertyBlock p = new MaterialPropertyBlock();

        yield return StartCoroutine(AnimateShrink(m, p));
    }

    public IEnumerator MakeGemDissapearIntoBlackHole(bool animate, Vector3 blackHoleLocation)
    {
        MeshRenderer m = gameObject.GetComponent<MeshRenderer>();
        MaterialPropertyBlock p = new MaterialPropertyBlock();

        float maxDuration = 0.5f;
        float duration = 0.0f;
        float targetValue = .5f;

        Vector3Int start = Address;

        while (duration < maxDuration)
        {
            transform.localPosition = Vector3.Lerp(start, blackHoleLocation, duration / maxDuration);
            p.SetFloat("_Radius", targetValue * (1.0f - (duration / maxDuration)));
            p.SetFloat("_Shape", (int)myType);
            p.SetColor("_Color", GetColor());
            m.SetPropertyBlock(p);
            //            print(myType);
            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return StartCoroutine(AnimateShrink(m, p));
    }


    public IEnumerator AnimateOversize(MeshRenderer m, MaterialPropertyBlock p)
    {
        float maxDuration = 0.5f;
        float duration = 0.0f;
        float targetValue = .5f;

        while (duration < maxDuration)
        {
            p.SetFloat("_Radius", targetValue * (0.5f - Mathf.Abs(0.5f - duration)));
            m.SetPropertyBlock(p);

            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator AnimateShrink(MeshRenderer m, MaterialPropertyBlock p)
    {
        float maxDuration = 0.5f;
        float duration = 0.0f;
        float targetValue = .5f;

        while (duration < maxDuration)
        {
            p.SetFloat("_Radius", targetValue * (1.0f - (duration / maxDuration)));
            p.SetFloat("_Shape", (int)myType);
            p.SetColor("_Color", GetColor());
            m.SetPropertyBlock(p);
//            print(myType);
            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


    public IEnumerator AnimateGrow(MeshRenderer m, MaterialPropertyBlock p)
    {
        float maxDuration = 1.0f;
        float duration = 0.0f;
        float targetValue = 0.5f;

        while (duration < maxDuration)
        {
            p.SetFloat("_Radius", targetValue * (duration / maxDuration));
            m.SetPropertyBlock(p);

            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void ChangeGemTypeAppearance(MeshRenderer m, MaterialPropertyBlock p)
    {
        UpdateColor(p);
        UpdateShape(p);
        m.SetPropertyBlock(p);
    }

    public void UpdateShape(MaterialPropertyBlock p)
    {
        p.SetFloat("_Shape", (int)myType);
    }

    public void UpdateColor(MaterialPropertyBlock p)
    {
        p.SetColor("_Color", GetColor());
    }

    public Color GetColor()
    {
        return GemColors[myType];
    }
}
