//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Plain : MonoBehaviour
{
    
    public int[][] coordsObjIdentifier;
    public int width, height;
    //contains a 2d location int array,
    //-1 represents open space,
    //0 represents floor tiles
    //1 represents wall tiles
    //2 represents monster spawn
    //3 represents loot spawn


    protected void buildPlain()
    {
        if(width > 0 && height > 0)
        {
            coordsObjIdentifier = new int[width][];
            for (int i = 0; i < width; i++)
            {
                coordsObjIdentifier[i] = new int[height];
                for(int j = 0; j < height; j++)
                    coordsObjIdentifier[i][j] = -1;
            }

           
        }
    }

}
