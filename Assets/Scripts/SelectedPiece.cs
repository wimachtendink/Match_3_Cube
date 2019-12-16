using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//this is a bit of a mess, and I use this kind of thing often, perhaps I shuld try to make a good version of it for future use

public class SelectedPiece : MonoBehaviour
{
    public GemSwapper swapper;
    public ButtonManager ButtonManager;

    public int CurrentChain = 0;

    public SoundPlayer FireSounds;
    public LineRenderer lr_Prefab;
    
    public Gem selected;

    public bool controllerIsLocked = false;

    public GameObject Indicator;

    public ControllerPlayspaceLocation selectionLocation;
    public SpaceMaker sm;

    public UnityEvent onScoreIncriment;
    
    public Vector3Int fallDirection = Vector3Int.down;

    public GameObject FallDirectionIndicator;

    private void Start()
    {
        SetFallIndicator();
    }

    //this is the only method which actually logically belongs in this script, Everything else should move I think
    public void OnIndicatorClick()
    {
        if(!ButtonManager.CheckForMenuButtonClick() && sm.ThereIsAWorld)
        {
            if (!controllerIsLocked)
            {
                FireSounds.PlaySound(FireSounds.Click);
                Gem tempGem;

                if (sm.addressBook.ContainsKey(selectionLocation.controllerPlayspacePostion))
                {
                    tempGem = sm.addressBook[selectionLocation.controllerPlayspacePostion];
                    //if we already have something selected
                    if (selected)
                    {
                        //then we check if it is the same thing as we ALREADY HAD SELECTED
                        if (selected != tempGem)
                        {
                            //Get the vector from one to the other
                            Vector3Int test = selected.Address - tempGem.Address;

                            //check if we selected something in a valid swap direction
                            if (Gem.directions.Contains(test))
                            {
                                Indicator.SetActive(false);
                                swapper.TrySwapGems(tempGem, selected, fallDirection);
                            }
                            else
                            {

                                //TODO: actually do this part!
                                Debug.LogError("Invalid_Selection.wav not found!");
                            }

                            selected = null;
                        }
                        else
                        {
                            Indicator.SetActive(false);
                            selected = null;
                        }

                        //TODO: consider then change, set selected to null here tautological-like!
                    }
                    else
                    {
                        //place indicator
                        Indicator.SetActive(true);
                        Indicator.transform.localPosition = tempGem.Address;
                        //set selected
                        selected = tempGem;
                    }
                }
                else
                {
//                    Debug.Log("setting fall direction - or trying to");
                    //should be get distance to center > (dimentions), set to dir.floortotin
                    if (selectionLocation.controllerPlayspacePostion.x > sm.dimentions)
                    {
                        fallDirection = new Vector3Int(1, 0, 0);
                    }
                    else if (selectionLocation.controllerPlayspacePostion.x < 0)
                    {
                        fallDirection = new Vector3Int(-1, 0, 0);
                    }
                    else if (selectionLocation.controllerPlayspacePostion.y > sm.dimentions)
                    {
                        fallDirection = new Vector3Int(0, 1, 0);
                    }
                    else if (selectionLocation.controllerPlayspacePostion.y < 0)
                    {
                        fallDirection = new Vector3Int(0, -1, 0);
                    }
                    else if (selectionLocation.controllerPlayspacePostion.z > sm.dimentions)
                    {
                        fallDirection = new Vector3Int(0, 0, 1);
                    }
                    else if (selectionLocation.controllerPlayspacePostion.z < 0)
                    {
                        fallDirection = new Vector3Int(0, 0, -1);
                    }

                    SetFallIndicator();

                    FireSounds.PlaySound(FireSounds.ChangeFallDirection);

                    //remove indicator if present
                    Indicator.SetActive(false);
                    //invalid selection
                    selected = null;
                }
            }
            else
            {
                Debug.LogError("controller_currently_locked.wav not found");
            }

        }
        else
        {
            Debug.LogWarning("we don't have a world but we are clicking!");
        }



    }

    private void SetFallIndicator()
    {
        FallDirectionIndicator.transform.localPosition = 
            (new Vector3Int(sm.dimentions/2, sm.dimentions / 2, sm.dimentions / 2)) + ( fallDirection * (sm.dimentions + 2));
    }
}
//Gem swapping!

//public void TrySwapGems(Gem g1, Gem g2)
//{
//    FireSounds.PlaySound(FireSounds.Swap);
//    StartCoroutine(DealWithSwap(g1, g2));
//}

//IEnumerator DealWithSwap(Gem g1, Gem g2)
//{
//    CurrentChain = 0;

//    controllerIsLocked = true;

//    //this is probably a terrible use of memory
//    Dictionary<Vector3Int, Gem> fakeWorld = new Dictionary<Vector3Int, Gem>(sm.addressBook);
//    List<Gem> removeList = new List<Gem>(SpaceMaker.DimentionsCube);

//    //set hypothetical swap
//    fakeWorld[g1.Address] = g2;
//    fakeWorld[g2.Address] = g1;
//    //change this to only check the rays on which the gems actually sit
//    Checker.CheckWorldForMatch3(fakeWorld, removeList, new List < List < Vector3Int >>());

//    if (removeList.Contains(g1) || removeList.Contains(g2))
//    {
//        //update positions
//        sm.addressBook[g1.Address] = g2;
//        sm.addressBook[g2.Address] = g1;

//        //update addresses to match positions
//        Vector3Int swapAddress = g1.Address;
//        g1.Address = g2.Address;
//        g2.Address = swapAddress;

//        //swap model locations
//        yield return StartCoroutine(Gem.AnimateDoubleSwap(g1, g2, true));
//        //run chains!
//        yield return StartCoroutine(RunChain(sm.addressBook, true));
//    }
//    else
//    {
//        //swap
//        yield return StartCoroutine(Gem.AnimateDoubleSwap(g1, g2, false));//this should swap

//        FireSounds.PlaySound(FireSounds.Unswap);
//        //swap again because the first one was bad
//        yield return StartCoroutine(Gem.AnimateDoubleSwap(g1, g2, true));//this should swap
//    }

//    //return controll to user

//    controllerIsLocked = false;
//}

//public IEnumerator RunChain(Dictionary<Vector3Int, Gem> world, bool animate)
//{
//    List<Gem> dropList = new List<Gem>();

//    List<LineRenderer> lrs = new List<LineRenderer>();
//    List<Gem> ListOfMatched = new List<Gem>(SpaceMaker.DimentionsCube);
//    List<List<Vector3Int>> listOfLines = new List<List<Vector3Int>>();
//    LineRenderer lr;
//    bool done = false;
//    Coroutine coroutine;

//    //this is getting a little ugly, it might be nice to modulalize some of this
//    while (!done)
//    {
//        Checker.CheckWorldForMatch3(world, ListOfMatched, listOfLines);
//        //if we have some things,
//        if(ListOfMatched.Count > 0)
//        {
//            //animate some stuff
//            if (animate)
//            {
//                float clipLen = 0.0f;
//                for (int l = 0; l < listOfLines.Count; l++)
//                {
//                    CurrentChain++;
//                    FireSounds.PlayDoopDoop(CurrentChain, out clipLen);

//                    onScoreIncriment.Invoke();

//                    //make a pretty little line renderer
//                    lr = Instantiate(lr_Prefab, sm.transform);

//                    lr.positionCount = 3;
//                    //todo:make line scale with playspace
//                    lr.widthMultiplier = sm.transform.localScale.magnitude * 0.1f;

//                    for (int i = 0; i < listOfLines[l].Count; i++)
//                    {
//                        lr.SetPosition(i, listOfLines[l][i]);
//                    }
//                    lrs.Add(lr);
//                    yield return new WaitForSeconds(clipLen/ CurrentChain);
//                }

//                FireSounds.source.pitch = 1;

//                for (int i = 0; i < lrs.Count; i++)
//                {
//                    DestroyImmediate(lrs[i].gameObject);
//                }

//                lrs.Clear();

//                for (int i = 0; i < listOfLines.Count; i++)
//                {
//                    listOfLines[i].Clear();
//                }
//                listOfLines.Clear();
//            }


//            //TODO:make this whole thing if(animate)
//            //disapear each gem
//            for (int i = 0; i < ListOfMatched.Count; i++)
//            {
//                //TO_maybe_DO: offset gems in line by some small amount - random number? - "heavy colors"?
//                coroutine = StartCoroutine(ListOfMatched[i].MakeGemDissapearIntoBlackHole(animate, FallDirectionIndicator.transform.localPosition));
//                if (animate && i == ListOfMatched.Count - 1)
//                {
//                    FireSounds.PlayCelebrationSound();
//                    yield return coroutine;
//                }
//            }

//            for (int i = 0; i < ListOfMatched.Count; i++)
//            {
//                int counter = 1;
//                bool inBounds = true;

//                Vector3Int startAddress = ListOfMatched[i].Address;
//                Vector3Int offset = fallDirection;

//                while(inBounds)
//                {
//                    Gem my = ListOfMatched[i];

//                    Vector3Int neighbourAddress = startAddress - (offset * counter);

//                    if (neighbourAddress.x < sm.dimentions &&
//                        neighbourAddress.y < sm.dimentions &&
//                        neighbourAddress.z < sm.dimentions &&
//                        neighbourAddress.x >= 0 &&
//                        neighbourAddress.y >= 0 &&
//                        neighbourAddress.z >= 0)
//                    {
//                        //address is in bounds
//                        if (ListOfMatched.Contains(world[neighbourAddress]))
//                        {
//                            //ignore things that are already moving down?
//                        }
//                        else
//                        {

//                            //if(dropList.Contains(world[neighbourAddress]))
//                            //{
//                            //    //s += " it looks like we're already dropping this one";
//                            //}

//                            Gem neighbour = world[neighbourAddress];

//                            neighbour.Address = my.Address;
//                            my.Address = neighbourAddress;

//                            world[my.Address] = my;
//                            world[neighbour.Address] = neighbour;

//                            //add to list for later animation
//                            dropList.Add(neighbour);
//                        }
//                    }
//                    else
//                    {
//                        //s += " but it was out of bounds, so we rejected it";
//                        inBounds = false;
//                    }

//                    //s += counter + "";
//                    //print(s);
//                    counter++;
//                }
//            }


//            for(int d = 0; d < dropList.Count ;d++)
//            {
//                coroutine = StartCoroutine(dropList[d].AnimateDrop());
//                if(d == dropList.Count -1 && animate)
//                {
//                    yield return coroutine;
//                }
//            }

//            dropList.Clear();

//            Vector3Int wayUp = fallDirection * -sm.dimentions;

//            foreach(var g in ListOfMatched)
//            {

//                //-down * dimentions should give me 1 above the world... right?

//                Vector3Int start = g.Address + wayUp;

//                StartCoroutine(g.AnimateReAppear());
//                StartCoroutine(g.AnimateDeal(start));
//            }

//        }
//        else
//        {
//            done = true;
//        }
//        ListOfMatched.Clear();
//    }
//    FireSounds.PlaySound(FireSounds.ControllerUnlock);
//}


