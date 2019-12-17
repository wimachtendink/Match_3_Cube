using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checker : MonoBehaviour
{

    public static Checker instance;

    //[SerializeField]
    public UnityEvent NoRemainingMoves;
    private void Start()
    {
        instance = this;
    }

    public const int RUN_LENGTH = 3;
    public  GemType GetBestSwapToColor(Gem g, Dictionary<Vector3Int, Gem> world)
    {

        GemType ReturnValue = GemType.none;

        List<Vector3Int> CheckDirs = new List<Vector3Int>(Gem.directions);

        Dictionary<GemType, int> derp = new Dictionary<GemType, int>((int)GemType.none - 1);
        foreach(var key in Gem.GemColors.Keys)
        {
            derp[key] = 0;
        }

        foreach (var dir in CheckDirs)
        {
            if (world.ContainsKey(g.Address + dir))
            {
                derp[world[g.Address + dir].myType] += 1;
            }
        }

//        string s = "";

        foreach(var pair in derp)
        {
            //s += " " + pair;
        }

        int currentBest = int.MaxValue;

        foreach(var pair in derp)
        {
            if(pair.Value < currentBest)
            {
                ReturnValue = pair.Key;
                currentBest = pair.Value;
            }
        }

        return ReturnValue;

    }

    /// <summary>
    /// True if the gem is not at the edge of the world, and 
    /// </summary>
    /// <param name="gemToCheck">the gem to check for</param>
    /// <param name="myGemType"></param>
    /// <param name="directions"></param>
    /// <param name="world"></param>
    /// <returns></returns>
    public  bool CheckForMatchWithNeighbor(Gem gemToCheck, GemType myGemType, List<Vector3Int> directions, Dictionary<Vector3Int, Gem> world)
    {
        foreach(var dir in directions)
        {
            // TODO: ADD TEST - verify that this can't be prematurely true
            // TODO: add a bunch of tests, since i'm only dealing with a ray at a time, it should be somewhat trivial
            if(world.ContainsKey(gemToCheck.Address + dir) && world[gemToCheck.Address + dir].myType == myGemType)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///  This uses some bit twiddling techniques to check the entire world for any matches
    /// </summary>
    /// <param name="StartingPosition">the first gem in a row (think face gems)</param>
    /// <param name="DirectionVector">the direction from the first to the second gem (and the direction of procession)</param>
    /// <param name="worldList">the copy of all gems in the world</param>
    /// <param name="listOfGemsToRemove">the globally unique set of gems to be removed in this turn</param>
    /// <param name="matchedLines">the list of lines that have been matched (for UI)</param>
    public  void BitwiseRayTester
        (
            Vector3Int StartingPosition,
            Vector3Int DirectionVector,
            Dictionary<Vector3Int, Gem> worldList,
            ref List<Gem> listOfGemsToRemove,
            ref List<List<Vector3Int>> matchedLines,
            ref bool _thereIsAMove
        )
    {
        //just getting all the directions?
        List<Vector3Int> checkDirs = new List<Vector3Int>(Gem.directions);

        //then we get rid of the ones we don't want - why don't we want these? because we know that they could not have been matched
        checkDirs.Remove(DirectionVector);
        checkDirs.Remove(DirectionVector * -1);

        //each color gets it's own collision mask which is a bool representation of if that color is present in the row
        int[] collisionMasks = new int[Gem.GemColors.Count];       

        for(int i = 0; i < collisionMasks.Length; i++)
        {
            //this is a place-saving mask to prevent 0's from not getting shifted
            collisionMasks[i] = 1;
        }

        //000001 is the number 1, and C#/Unity will remove all those leading zeros which makes it useless as a mask
        //the hack here is to add a leading 1 which preserves the structure, but really a bool arr or list would probably be fine
            //I think I was just bit-twiddling as an excercise here since I didn't even benchmark anything
        int lastI = 0;
        for (int i = 0; i < SpaceMaker.Dimentions; i++)
        {
            //incriment by 1 so we have the same leading values - limits max dimention length to 31 which is plenty
            for (int idx = 0; idx < collisionMasks.Length; idx++)
            {
                collisionMasks[idx] <<= 1;
            }
            collisionMasks[(int) worldList[StartingPosition + (DirectionVector * i)].myType] += 1;
            lastI = i;
        }

        //string s = "";        

        int placeholderBitValue = 1 << (lastI + 1);
        for (int idx = 0; idx < collisionMasks.Length; idx++)
        {
            //s += Convert.ToString(collisionMasks[idx], toBase:2) + " " + (GemType)idx + "\n";
            collisionMasks[idx] -= placeholderBitValue;
        }

        //we can ignore a few checks by limiting to dim - (runlen-1)
        for (int i = 0; i < SpaceMaker.Dimentions - (RUN_LENGTH - 1); i++)
        {
            //bitwise compare each color to check for a run of RUN_LENGTH
            for(int c = 0; c < Gem.GemColors.Count; c++)
            {
                int temp = (collisionMasks[c] & 7);
                if (temp == 7)
                {
                    AddToListIfNotInList(worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (1 + i))))], listOfGemsToRemove);
                    AddToListIfNotInList(worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (2 + i))))], listOfGemsToRemove);
                    AddToListIfNotInList(worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (3 + i))))], listOfGemsToRemove);

                    matchedLines.Add(new List<Vector3Int>()
                    {
                        StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (1 + i))),
                        StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (2 + i))),
                        StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (3 + i))),
                    });
                }
                //only checking for moves if we haven't already found one because at the moment we don't care how many moves there are, just if there is at least one
                else if(!_thereIsAMove)
                {
                    Gem SwapMe = null;
                    bool checkForNeighbour = false;
                    GemType SwapMeWith = GemType.none;

                    if (temp == 6)
                    {
                        //this case is actually that the FIRST is the one to check because of FILO
                        //110
                        checkForNeighbour = true;
                        //debugv3 = (StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (1 + i))));
                        SwapMe = worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (1 + i))))];
                        SwapMeWith = worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (2 + i))))].myType;
                    }
                    else if (temp == 5)
                    {
                        //101
                        checkForNeighbour = true;
                        //debugv3 = (StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (2 + i))));
                        SwapMe = worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (2 + i))))];
                        SwapMeWith = worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (1 + i))))].myType;
                    }
                    else if (temp == 3)
                    {
                        //011
                        checkForNeighbour = true;
                        //debugv3 = (StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (3 + i))));
                        SwapMe = worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (3 + i))))];
                        SwapMeWith = worldList[(StartingPosition + (DirectionVector * (SpaceMaker.Dimentions - (2 + i))))].myType;
                    }
                    if(checkForNeighbour)
                    {
                        _thereIsAMove = CheckForMatchWithNeighbor(SwapMe, SwapMeWith, checkDirs, worldList);
                    }
                }
                // TODO: (unsure) moving everything over one to check the next place value
                // TODO: checking for 011 could allow us to incriment by 2?
                int leftAmount = 2;
                //this might be backwards and we need to check 6?
                if(temp == 3)
                {
                    leftAmount = 1;
                }
                collisionMasks[c] >>= leftAmount;
            }
        }
    }
    
    public  bool CompareGemTypes(Gem g1, Gem g2)
    {
        return g1.myType == g2.myType;
    }

    // TODO: make a "check gem for matches" which will be faster for checking swaps
    public  void CheckWorldForMatch3(Dictionary<Vector3Int, Gem> world, 
                                            List<Gem> removeList, 
                                            List<List<Vector3Int>> lineAddressesList, 
                                            bool CheckForMoves)
    {

        removeList.Clear();
        bool thereIsAMove = !CheckForMoves;
        int debugCount;

        //for each ray pointing inward (and their... [rotational partners?]) check for matches
        for (int x = 0; x < SpaceMaker.Dimentions; x++)
        {
            for (int yz = 0; yz < SpaceMaker.Dimentions; yz++)
            {
                BitwiseRayTester(new Vector3Int(x, yz, 0), new Vector3Int(0, 0, 1), world, ref removeList, ref lineAddressesList, ref thereIsAMove);
                debugCount = lineAddressesList.Count;

                BitwiseRayTester(new Vector3Int(x, 0, yz), new Vector3Int(0, 1, 0), world, ref removeList, ref lineAddressesList, ref thereIsAMove);
                debugCount = lineAddressesList.Count - debugCount;
                
                BitwiseRayTester(new Vector3Int(0, x, yz), new Vector3Int(1, 0, 0), world, ref removeList, ref lineAddressesList, ref thereIsAMove);
                debugCount = lineAddressesList.Count - debugCount;
                
            }
        }
        if(!thereIsAMove)
        {
            print("no move was found, which means this move will end the game!!");
            NoRemainingMoves.Invoke();
        }
    }

    public  void AddToListIfNotInList<T>(T thing, List<T> list)
    {
        if (!list.Contains(thing))
        {
            list.Add(thing);
        }
    }
}

