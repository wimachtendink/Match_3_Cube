using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public const int RUN_LENGTH = 3;
    public static bool ThereIsAMove = false;
    public static GemType GetBestSwapToColor(Gem g, Dictionary<Vector3Int, Gem> world)
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

    public static bool CheckForNeighbour(Gem g, GemType checkType, List<Vector3Int> directions, Dictionary<Vector3Int, Gem> world)
    {
        foreach(var dir in directions)
        {            
            if(world.ContainsKey(g.Address + dir) && world[g.Address + dir].myType == checkType)
            {
                return true;
            }
        }
        return false;
    }

    public static void BitwiseRayTester(Vector3Int sPos,
                                    Vector3Int dirVec,
                                    Dictionary<Vector3Int, Gem> w,
                                    ref List<Gem> outList,
                                    ref List<List<Vector3Int>> lines)
    {

        Vector3 debugv3 = Vector3.zero;
        //Debug.Log("we are starting Bitwise RayTester!");

        List<Vector3Int> checkDirs = new List<Vector3Int>(Gem.directions);
//        print(checkDirs[0]);

        checkDirs.Remove(dirVec);
        checkDirs.Remove(dirVec * -1);

        int[] collisionMasks = new int[Gem.GemColors.Count];       

        for(int i = 0; i < collisionMasks.Length; i++)
        {
            //this is a place-saving mask to prevent 0's from not getting shifted
            collisionMasks[i] = 1;
        }

        int lastI = 0;
        for (int i = 0; i < SpaceMaker.Dimentions; i++)
        {
            //incriment by 1 so we have the same leading values - limits max dimention length to 31 which is plenty
            for (int idx = 0; idx < collisionMasks.Length; idx++)
            {
                collisionMasks[idx] <<= 1;
            }
            collisionMasks[(int) w[sPos + (dirVec * i)].myType] += 1;
            lastI = i;
        }

        //string s = "";        

        int placeholderBitValue = 1 << (lastI + 1);
        for (int idx = 0; idx < collisionMasks.Length; idx++)
        {
            //s += Convert.ToString(collisionMasks[idx], toBase:2) + " " + (GemType)idx + "\n";
            collisionMasks[idx] -= placeholderBitValue;
        }

        for (int i = 0; i < SpaceMaker.Dimentions - (RUN_LENGTH - 1); i++)
        {
            //print(i);
            for(int c = 0; c < Gem.GemColors.Count; c++)
            {
                int temp = (collisionMasks[c] & 7);
                if (temp == 7)
                {
                    AddToListIfUnique(w[(sPos + (dirVec * (SpaceMaker.Dimentions - (1 + i))))], outList);
                    AddToListIfUnique(w[(sPos + (dirVec * (SpaceMaker.Dimentions - (2 + i))))], outList);
                    AddToListIfUnique(w[(sPos + (dirVec * (SpaceMaker.Dimentions - (3 + i))))], outList);

                    lines.Add(new List<Vector3Int>()
                    {
                        sPos + (dirVec * (SpaceMaker.Dimentions - (1 + i))),
                        sPos + (dirVec * (SpaceMaker.Dimentions - (2 + i))),
                        sPos + (dirVec * (SpaceMaker.Dimentions - (3 + i))),
                    });
                }
                else if(!ThereIsAMove)
                {
                    Gem SwapMe = null;
                    bool checkForNeighbour = false;
                    GemType SwapMeWith = GemType.none;

                    if (temp == 6)
                    {
                        //this case is actually that the FIRST is the one to check because of FILO
                        //110
                        checkForNeighbour = true;
                        debugv3 = (sPos + (dirVec * (SpaceMaker.Dimentions - (1 + i))));
                        SwapMe = w[(sPos + (dirVec * (SpaceMaker.Dimentions - (1 + i))))];
                        SwapMeWith = w[(sPos + (dirVec * (SpaceMaker.Dimentions - (2 + i))))].myType;
                    }
                    else if (temp == 5)
                    {
                        //101
                        checkForNeighbour = true;
                        debugv3 = (sPos + (dirVec * (SpaceMaker.Dimentions - (2 + i))));
                        SwapMe = w[(sPos + (dirVec * (SpaceMaker.Dimentions - (2 + i))))];
                        SwapMeWith = w[(sPos + (dirVec * (SpaceMaker.Dimentions - (1 + i))))].myType;

                        
                    }
                    else if (temp == 3)
                    {
                        //011
                        checkForNeighbour = true;
                        debugv3 = (sPos + (dirVec * (SpaceMaker.Dimentions - (3 + i))));
                        SwapMe = w[(sPos + (dirVec * (SpaceMaker.Dimentions - (3 + i))))];
                        SwapMeWith = w[(sPos + (dirVec * (SpaceMaker.Dimentions - (2 + i))))].myType;

                    }
                    if(checkForNeighbour)
                    {
                        ThereIsAMove = CheckForNeighbour(SwapMe, SwapMeWith, checkDirs, w);
                    }
                }      
                collisionMasks[c] >>= 1;
            }
        }
    }
    
    public static bool CheckGems(Gem g1, Gem g2)
    {
        return g1.myType == g2.myType;
    }

    //I should also make a "check gem for matches" which will be faster for checking swaps
    public static void CheckWorldForMatch3(Dictionary<Vector3Int, Gem> world, 
                                            List<Gem> removeList, 
                                            List<List<Vector3Int>> lineAddressesList, 
                                            bool CheckForMoves)
    {

        removeList.Clear();
        ThereIsAMove = !CheckForMoves;
        int debugCount;

        //Debug.Log("starting CheckWorldForMatch3");

        for (int x = 0; x < SpaceMaker.Dimentions; x++)
        {
            for (int yz = 0; yz < SpaceMaker.Dimentions; yz++)
            {

                //something is wrong here, we're returning crazy lines
//                Debug.Log("doing bitwise ray tester");
                BitwiseRayTester(new Vector3Int(x, yz, 0), new Vector3Int(0, 0, 1), world, ref removeList, ref lineAddressesList);
                debugCount = lineAddressesList.Count;

                BitwiseRayTester(new Vector3Int(x, 0, yz), new Vector3Int(0, 1, 0), world, ref removeList, ref lineAddressesList);
                debugCount = lineAddressesList.Count - debugCount;
                
                BitwiseRayTester(new Vector3Int(0, x, yz), new Vector3Int(1, 0, 0), world, ref removeList, ref lineAddressesList);
                debugCount = lineAddressesList.Count - debugCount;
                
            }
        }
        if(!ThereIsAMove)
        {
            print("no move was found, which means this move will end the game!!");
        }
    }

    public static void AddToListIfUnique(Gem g, List<Gem> l)
    {
        if (!l.Contains(g))
        {
            l.Add(g);
        }
    }




}



/*

public static void CheckWorldForMatch3_Old(Dictionary<Vector3Int, Gem> world, List<Gem> removeList, List<List<Vector3Int>> lineAddressesList, int MinClearSize )
{
    GemType checkType;
    removeList.Clear();

    //we want "for each Ray"
        //so we say foreach x and y

    //make a "check ray" method

    for (int x = 0; x < SpaceMaker.Dimentions; x++)
    {
        for (int yz = 0; yz < SpaceMaker.Dimentions; yz++)
        {
            //check first and last of each possible run
            checkType = world[new Vector3Int(x, yz, 0)].myType;
            if (checkType == world[new Vector3Int(x, yz, 2)].myType)
            {
                if (checkType == world[new Vector3Int(x, yz, 1)].myType)
                {

                    AddToListIfUnique(world[new Vector3Int(x, yz, 0)], removeList);
                    AddToListIfUnique(world[new Vector3Int(x, yz, 1)], removeList);
                    AddToListIfUnique(world[new Vector3Int(x, yz, 2)], removeList);

                    lineAddressesList.Add(new List<Vector3Int>()
                    {
                        new Vector3Int(x, yz, 0),
                        new Vector3Int(x, yz, 1),
                        new Vector3Int(x, yz, 2)
                    });
                }
            }

            checkType = world[new Vector3Int(x, 0, yz)].myType;

            if (checkType == world[new Vector3Int(x, 2, yz)].myType)
            {
                if (checkType == world[new Vector3Int(x, 1, yz)].myType)
                {
                    AddToListIfUnique(world[new Vector3Int(x, 0, yz)], removeList);
                    AddToListIfUnique(world[new Vector3Int(x, 1, yz)], removeList);
                    AddToListIfUnique(world[new Vector3Int(x, 2, yz)], removeList);

                    lineAddressesList.Add(new List<Vector3Int>()
                    {
                        new Vector3Int(x, 0, yz),
                        new Vector3Int(x, 1, yz),
                        new Vector3Int(x, 2, yz)
                    });
                }
            }

            checkType = world[new Vector3Int(0, x, yz)].myType;

            if (checkType == world[new Vector3Int(2, x, yz)].myType)
            {
                if (checkType == world[new Vector3Int(1, x, yz)].myType)
                {
                    AddToListIfUnique(world[new Vector3Int(0, x, yz)], removeList);
                    AddToListIfUnique(world[new Vector3Int(1, x, yz)], removeList);
                    AddToListIfUnique(world[new Vector3Int(2, x, yz)], removeList);

                    lineAddressesList.Add(new List<Vector3Int>()
                    {
                        new Vector3Int(0, x, yz),
                        new Vector3Int(1, x, yz),
                        new Vector3Int(2, x, yz)
                    });
                }
            }
        }
    }
}



//this could be renamed(check for move OR Run) and could do both since I'm going to have to check everything anyway
//public static void CheckForMove(List<Gem> gems)
//{
//    List<Vector3Int> directions = new List<Vector3Int>(Gem.directions);

//    byte b = 0;

//    Vector3Int dir = gems[0].Address - gems[1].Address;

//    print("checking " + dir + " ray ");

//    directions.Remove(dir);
//    directions.Remove(dir * -1);


//    if (gems[0].myType == gems[1].myType)
//    {
//        b += 1;
//    }

//    if (gems[0].myType == gems[2].myType)
//    {
//        b += 2;
//    }

//    if (gems[1].myType == gems[2].myType)
//    {
//        b += 4;
//    }


//    //1: a=b
//    //2: a=c
//    //4: b=c

//    switch (b)
//    {
//        case 1:
//            //a=b, check for a c neighbour!
//            break;
//        case 2:
//            //a = c, check for b neighbours
//            break;
//        case 4:
//            //b=c, check for a neighbours
//            break;
//        default:
//            break;
//            //this is a match
//    }        
//}





*/



/*

public static void CheckRay(Vector3Int sPos, 
                                Vector3Int dirVec, 
                                Dictionary<Vector3Int, Gem> w,
                                ref List<Gem> outList,
                                ref List<List<Vector3Int>> lines,
                                int R)
{     
    for (int n = 0; n * R < SpaceMaker.Dimentions; n++)
    {
        int nR = n * R;

        //make sure we won't go out of bounds negative
        //X?X
        if(nR > 0 && nR + 1 < SpaceMaker.Dimentions)
        {
            //if these match, we only need to check the middle
            if(CheckGems(w[sPos + (dirVec * (nR - 1))], w[sPos + (dirVec * (nR + 1))]))
            {
                if (CheckGems(w[sPos + (dirVec * (nR))], w[sPos + (dirVec * (nR + 1))]))
                {

                    //nR-1, nr, and nR+1 are a line and should be added
                    AddToListIfUnique(w[sPos + dirVec * (nR - 1)]   , outList);
                    AddToListIfUnique(w[sPos + dirVec * (nR)]       , outList);
                    AddToListIfUnique(w[sPos + dirVec * (nR + 1)]   , outList);

                    lines.Add(new List<Vector3Int>()
                    {
                        sPos + dirVec * (nR - 1),
                        sPos + dirVec * (nR),
                        sPos + dirVec * (nR+1)
                    });
                }
            }
        }

        //?XX

        if(nR + 2 < SpaceMaker.Dimentions)
        {
            //?XX?
            if(CheckGems(w[sPos + (dirVec * (nR + 1))], w[sPos + (dirVec * (nR + 2))]))
            {
                //XXX?
                //check nr
                if (CheckGems(w[sPos + (dirVec * (nR))], w[sPos + (dirVec * (nR + 1))]))
                {
                    AddToListIfUnique(w[sPos + dirVec * (nR)]   , outList);
                    AddToListIfUnique(w[sPos + dirVec * (nR+1)] , outList);
                    AddToListIfUnique(w[sPos + dirVec * (nR+2)] , outList);
                    //add one to lines...
                    //or make a new line I guess
                    //nR-1, nr, and nR+1 are a line and should be added

                    lines.Add(new List<Vector3Int>()
                    {
                        sPos + dirVec * nR,
                        sPos + dirVec * (nR+1),
                        sPos + dirVec * (nR+2),
                    });

                }

                //?XXX
                //TODO: rethink this
                if (nR + 3 < SpaceMaker.Dimentions)
                {
                    if (CheckGems(w[sPos + (dirVec * (nR + 3))], w[sPos + (dirVec * (nR + 2))]))
                    {
                        AddToListIfUnique(w[sPos + dirVec * (nR + 1)], outList);
                        AddToListIfUnique(w[sPos + dirVec * (nR + 2)], outList);
                        AddToListIfUnique(w[sPos + dirVec * (nR + 3)], outList);

                        lines.Add(new List<Vector3Int>()
                        {
                            sPos + dirVec * (nR+1),
                            sPos + dirVec * (nR+2),
                            sPos + dirVec * (nR+3),
                        });
                    }
                }
            }
        }
    }
}


*/
