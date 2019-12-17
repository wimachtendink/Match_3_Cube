using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMaker : MonoBehaviour
{
    public ButtonValue dimButtonValue;
    public ButtonValue colorButtonValue;

    public bool ThereIsAWorld = false;

    public bool animateStartStuff;

    public GameObject GemPrefab;

    [Range(3,6)]
    public int qtyGemColors;

    public static int Dimentions;
    public static int DimentionsCube;

    [Range(3, 10)]
    public int dimentions;
    public Transform playspace;    
    public Dictionary<Vector3Int, Gem> addressBook;

    [Tooltip("This needs to be moved to somewhere more reasonable")]
    public SelectedPiece sp;


    public void CreateNewCubeSpace()
    {
        int _dim = dimButtonValue.value;
        qtyGemColors = colorButtonValue.value;

        if (addressBook != null)
        {
            Apocalypse();
        }

        Dimentions = _dim;
        dimentions = _dim;
        DimentionsCube = _dim * _dim * _dim;

        FillSpaceWithGems(_dim, qtyGemColors);

        RemoveRuns();
        AnimateAppearAllGems();

        ThereIsAWorld = true;

    }

    void AnimateAppearAllGems()
    {
        foreach(var g in addressBook.Values)
        {
            g.AnimateAppear();
        }
    }

    void RemoveRuns()
    {
        List<Gem> list = new List<Gem>(DimentionsCube);

        List<List<Vector3Int>> ll = new List<List<Vector3Int>>();

        //print(" --- Start! ---");

        Checker.CheckWorldForMatch3(addressBook, list, ll, false);

        int counter = 0;

//        print(ll.Count);
        
        while(list.Count > 0 && counter < 100)
        {
            foreach (var l in ll)
            {
                foreach(var v in l)
                {
                    addressBook[v].MakeNewGemOfType(Checker.GetBestSwapToColor(addressBook[v], addressBook));
                }
            }
            ll.Clear();

//            print(" --- CLEARED! ---");
            Checker.CheckWorldForMatch3(addressBook, list, ll, false);
            counter++;
        }

        if(counter == 100)
        {
            Debug.LogError("We never got settled in 100 runs!");
        }

//        print("ran " + counter + " times");
    }

    public void Apocalypse()
    {
        foreach(var pair in addressBook)
        {
            Destroy(pair.Value.gameObject);
        }

        addressBook = new Dictionary<Vector3Int, Gem>();
    }


    void FillSpaceWithGems(int _dim, int _qtyColors)
    {
        addressBook = new Dictionary<Vector3Int, Gem>(DimentionsCube);
        for (int x = 0; x < dimentions; x++)
        {
            for (int y = 0; y < dimentions; y++)
            {
                for (int z = 0; z < dimentions; z++)
                {
                    GameObject temp = Instantiate(GemPrefab, playspace);
                    temp.transform.localPosition = new Vector3(x, y, z);
                    temp.GetComponent<Gem>().Address = new Vector3Int(x, y, z);
                    addressBook.Add(temp.GetComponent<Gem>().Address, temp.GetComponent<Gem>());
                    temp.GetComponent<Gem>().MakeNewRandomGem(_qtyColors);
                }
            }
        }
    }
}
