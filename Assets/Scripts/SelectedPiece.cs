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

    public void OnIndicatorClick()
    {

        /*
        //isn't there a better way to do this? it seems crazy
        //reject enum:
        Menu
        NoWorld
        Locked
        OutOfBounds
        Deselect
        InvalidSwap

        // 1) check if we have clicked a button
        // 2) check if the world has been created
        // 3) check if the controller is locked
        // 4) check if we have selected a valid position
        // 5) check if we already have a selected piece
        // 6) check if we have reselected the same piece (deselect)
        // 7) check if the selected piece is a valid swap

        */

        if (!ButtonManager.CheckForMenuButtonClick())
        {
            if(sm.ThereIsAWorld)
            {
                HandleWorldClick();
            }
            else
            {
                Debug.LogWarning("we don't have a world but we are clicking!");
            }
        }else
        {
            //this is the "we clicked on a button" case which overrides world clicks
        }
    }

    void HandleWorldClick()
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
                            // TODO: TrySwap should return bool which should be handled here, what happens when it fails?
                            swapper.StartGemSwapAttempt(tempGem, selected, fallDirection);
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

    private void SetFallIndicator()
    {
        FallDirectionIndicator.transform.localPosition = 
            (new Vector3Int(sm.dimentions/2, sm.dimentions / 2, sm.dimentions / 2)) + ( fallDirection * (sm.dimentions + 2));
    }
}
