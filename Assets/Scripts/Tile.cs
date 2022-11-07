using OpenCover.Framework.Model;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // Start is called before the first frame update
    public int locationx, locationz;
    public int[] adjList = {-1, -1, -1, -1} ;//contains neighbor tile index -2 -> outside of bounds, -1 free, >= 0 -> neighbors index
    public int neighborCount;
    public int tileNumber;
    public bool isEdge = true;

    /*Tile checks its neighbors referencing its own position in
     * the room mask.
     * first the tile checks north tile, east tile, south tile, west tile in that order
     * for each position on the mask adjacent to this tile that is >= 0 it is added to the adjlist
     * at the postion it represent N,E,S,W. if the tile has 4 neighbors then it isnt an edge.
    */
    //returns true is any neighbors are no longer edges
    public void checkNeighbors(int[][] roomMask)
    {


        neighborCount = 0;
        for(int i = 0; i < adjList.Length; i++)
        {
            switch (i)
            {
                case 0://north
                    if(locationz != 0)//check if furthest north on map
                    {
                        if (roomMask[locationx][locationz - 1] != -1)
                        {
                            neighborCount++;
                            adjList[i] = roomMask[locationx][locationz - 1];
                        }
                    }
                    break;
                case 1://east
                    if (locationx != roomMask.Length - 1)//check if furthest east on map
                    {
                        if (roomMask[locationx + 1][locationz] != -1)
                        {
                            neighborCount++;
                            adjList[i] = roomMask[locationx + 1][locationz];
                        }
                    }
                    break;
                case 2://south
                    if (locationz != roomMask[0].Length - 1)//check if furthest south on map
                    {
                        if (roomMask[locationx][locationz + 1] != -1)
                        {
                            neighborCount++;
                            adjList[i] = roomMask[locationx][locationz + 1];
                        }
                    }
                    break;
                default://west
                    if (locationx != 0)//check if furthest west on map
                    {
                        if (roomMask[locationx - 1][locationz] != -1)
                        {
                            neighborCount++;
                            adjList[i] = roomMask[locationx - 1][locationz];
                        }
                    }
                    break;
            }
        }
        if(neighborCount < 4)
        {
            isEdge = true;
        }
        else
        {
            isEdge = false;
        }
    }




}
