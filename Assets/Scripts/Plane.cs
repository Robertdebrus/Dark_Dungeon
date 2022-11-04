//using System.Collections;
//using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



public class Plane : MonoBehaviour
{

    protected struct Pair
    {
        public int x;
        public int y;
    }
    
    [SerializeField] public int[][] coordsObjIdentifier;
    [SerializeField]public int width, height;
    [SerializeField] Pair furthestLeft, furthestRight, furthestTop, furthestBottom;//keeps track for resizing mask
    //contains a 2d location int array,
    //-1 represents open space,
    //0 represents floor tiles
    //1 represents wall tiles
    //2 represents monster spawn
    //3 represents loot spawn

    

    protected void buildPlane()
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


    protected void AutoResizePlane()
    {
        FindFurthest();//find the furthest tile positions on mask to resize
        int[][] newPlane;
        int newWidth = furthestRight.x - furthestLeft.x + 1;
        int newHeight = furthestBottom.y - furthestTop.y + 1;

        if (width != newWidth && height != newHeight)
        {
            newPlane = new int[newWidth][];

            Pair commonPoint;
            commonPoint.x = furthestLeft.x;
            commonPoint.y = furthestTop.x;

            for (int i = 0; i < newWidth; i++)
            {
                newPlane[i] = new int[newHeight];

                for (int j = 0; j < newHeight; j++)
                    newPlane[i][j] = coordsObjIdentifier[i + commonPoint.x][j + commonPoint.y];
            }

            this.width = newWidth;
            this.height = newHeight;
            this.coordsObjIdentifier = newPlane;

        }

        

    }

    private void FindFurthest()
    {
        //furthest points initially oposite point on mask they represent but set outside range
        //only need to set one of their pair values based on what they are looking for
        furthestRight.x = furthestBottom.y  = -1;//use min mask value as initial value for right and bottom
        furthestLeft.x = furthestTop.y = width;//use max mask value as initial value for top and left

        for(int i = 0; i < coordsObjIdentifier.Length; i++)//x
        {
            for(int j = 0; j < coordsObjIdentifier[i].Length; j++)//y
            {
                if(coordsObjIdentifier[i][j] >= 0)
                {
                    if(furthestLeft.x > i)
                    {
                        furthestLeft.x = i;
                        furthestLeft.y = j;
                    }

                    if(furthestRight.x < i)
                    {
                        furthestRight.x = i;
                        furthestRight.y = j;
                    }

                    if(furthestBottom.y < j)
                    {
                        furthestBottom.x = i;
                        furthestBottom.y = j;
                    }

                    if(furthestTop.y > j)
                    {
                        furthestTop.x = i;
                        furthestTop.y = j;
                    }
                }
            }
        }
    }

}
