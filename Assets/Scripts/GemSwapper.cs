using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// TODO: Generally break everything into smaller classes, then try to add come DI in lieu of DOTS

public class GemSwapper : MonoBehaviour
{
    public int CurrentChain = 0;

    public SoundPlayer FireSounds;
    public LineRenderer lr_Prefab;

    public Gem selected;

    public bool controllerIsLocked = false;

    public GameObject Indicator;

    public ControllerPlayspaceLocation selectionLocation;
    public SpaceMaker sm;

    public UnityEvent onScoreIncriment;

    public GameObject FallDirectionIndicator;

    //I would prefer if everything returned to this point
    public void StartGemSwapAttempt(Gem g1, Gem g2, Vector3Int _fallDirection)
    {
        FireSounds.PlaySound(FireSounds.Swap);
        StartCoroutine(DealWithSwap(g1, g2, _fallDirection));
    }

    IEnumerator DealWithSwap(Gem g1, Gem g2, Vector3Int _fallDirection)
    {
        CurrentChain = 0;

        controllerIsLocked = true;
        
        Dictionary<Vector3Int, Gem> fakeWorld = new Dictionary<Vector3Int, Gem>(sm.addressBook);
        List<Gem> removeList = new List<Gem>(SpaceMaker.DimentionsCube);

        //set hypothetical swap
        fakeWorld[g1.Address] = g2;
        fakeWorld[g2.Address] = g1;

        //change this to only check the rays on which the gems actually sit 
            // no, we need to check all rays that either gem sits on 
        Checker.instance.CheckWorldForMatch3(fakeWorld, removeList, new List<List<Vector3Int>>(), true);

        if (removeList.Contains(g1) || removeList.Contains(g2))
        {
            //update positions
            sm.addressBook[g1.Address] = g2;
            sm.addressBook[g2.Address] = g1;

            //update addresses to match positions
            Vector3Int swapAddress = g1.Address;
            g1.Address = g2.Address;
            g2.Address = swapAddress;

            //swap model locations
            yield return StartCoroutine(Gem.AnimateDoubleSwap(g1, g2, true));
            //run chains!
            yield return StartCoroutine(RunChain(sm.addressBook, true, _fallDirection));
        }
        else
        {
            //swap
            yield return StartCoroutine(Gem.AnimateDoubleSwap(g1, g2, false));//this should swap

            FireSounds.PlaySound(FireSounds.Unswap);
            //swap again because the first one was bad
            yield return StartCoroutine(Gem.AnimateDoubleSwap(g1, g2, true));//this should swap
        }
        controllerIsLocked = false;
    }

    public IEnumerator RunChain(Dictionary<Vector3Int, Gem> world, bool animate, Vector3Int _fallDirection)
    {
        List<Gem> dropList = new List<Gem>();

        List<LineRenderer> lrs = new List<LineRenderer>();
        List<Gem> ListOfMatched = new List<Gem>(SpaceMaker.DimentionsCube);
        List<List<Vector3Int>> listOfLines = new List<List<Vector3Int>>();
        LineRenderer lineRenderer;
        bool done = false;
        Coroutine coroutine;

        //this is getting a little ugly, it might be nice to modulalize some of this
        while (!done)
        {
            Checker.instance.CheckWorldForMatch3(world, ListOfMatched, listOfLines, true);
            //if we have some things,
            if (ListOfMatched.Count > 0)
            {
                //animate some stuff
                if (animate)
                {
                    float clipLen = 0.0f;
                    for (int l = 0; l < listOfLines.Count; l++)
                    {
                        CurrentChain++;
                        FireSounds.PlayDoopDoop(CurrentChain, out clipLen);

                        onScoreIncriment.Invoke();

                        //make a pretty little line renderer
                        lineRenderer = Instantiate(lr_Prefab, sm.transform);

                        lineRenderer.positionCount = 3;
                        //todo:make line scale with playspace
                        lineRenderer.widthMultiplier = sm.transform.localScale.magnitude * 0.1f;

                        for (int i = 0; i < listOfLines[l].Count; i++)
                        {
                            lineRenderer.SetPosition(i, listOfLines[l][i]);
                        }
                        lrs.Add(lineRenderer);
                        yield return new WaitForSeconds(clipLen / CurrentChain);
                    }

                    FireSounds.source.pitch = 1;

                    for (int i = 0; i < lrs.Count; i++)
                    {
                        DestroyImmediate(lrs[i].gameObject);
                    }

                    lrs.Clear();

                    for (int i = 0; i < listOfLines.Count; i++)
                    {
                        listOfLines[i].Clear();
                    }
                    listOfLines.Clear();
                }


                //TODO:make this whole thing if(animate)
                //disapear each gem
                for (int i = 0; i < ListOfMatched.Count; i++)
                {
                    //TO_maybe_DO: offset gems in line by some small amount - random number? - "heavy colors"?
                    coroutine = StartCoroutine(ListOfMatched[i].MakeGemDissapearIntoBlackHole(animate, FallDirectionIndicator.transform.localPosition));
                    if (animate && i == ListOfMatched.Count - 1)
                    {
                        FireSounds.PlayCelebrationSound();
                        yield return coroutine;
                    }
                }

                for (int i = 0; i < ListOfMatched.Count; i++)
                {
                    int counter = 1;
                    bool inBounds = true;

                    Vector3Int startAddress = ListOfMatched[i].Address;
                    Vector3Int offset = _fallDirection;

                    while (inBounds)
                    {
                        Gem my = ListOfMatched[i];

                        Vector3Int neighbourAddress = startAddress - (offset * counter);

                        if (neighbourAddress.x < sm.dimentions &&
                            neighbourAddress.y < sm.dimentions &&
                            neighbourAddress.z < sm.dimentions &&
                            neighbourAddress.x >= 0 &&
                            neighbourAddress.y >= 0 &&
                            neighbourAddress.z >= 0)
                        {
                            //address is in bounds
                            if (ListOfMatched.Contains(world[neighbourAddress]))
                            {
                                //ignore things that are already moving down?
                            }
                            else
                            {

                                //if(dropList.Contains(world[neighbourAddress]))
                                //{
                                //    //s += " it looks like we're already dropping this one";
                                //}

                                Gem neighbour = world[neighbourAddress];

                                neighbour.Address = my.Address;
                                my.Address = neighbourAddress;

                                world[my.Address] = my;
                                world[neighbour.Address] = neighbour;

                                //add to list for later animation
                                dropList.Add(neighbour);
                            }
                        }
                        else
                        {
                            //s += " but it was out of bounds, so we rejected it";
                            inBounds = false;
                        }

                        //s += counter + "";
                        //print(s);
                        counter++;
                    }
                }


                for (int d = 0; d < dropList.Count; d++)
                {
                    coroutine = StartCoroutine(dropList[d].AnimateDrop());
                    if (d == dropList.Count - 1 && animate)
                    {
                        yield return coroutine;
                    }
                }

                dropList.Clear();

                Vector3Int wayUp = _fallDirection * -sm.dimentions;

                foreach (var g in ListOfMatched)
                {
                    Vector3Int start = g.Address + wayUp;

                    StartCoroutine(g.AnimateReAppear());
                    StartCoroutine(g.AnimateDeal(start));
                }
            }
            else
            {
                done = true;
            }
            ListOfMatched.Clear();
        }
        FireSounds.PlaySound(FireSounds.ControllerUnlock);
    }
}
