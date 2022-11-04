using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // Start is called before the first frame update
    public int locationx, locationz;
    public int[] adjList = {-1, -1, -1, -1} ;//contains neighbor tile index -2 -> outside of bounds, -1 free, >= 0 -> neighbors index
    public int neighborCount = 0;
    public int tileNumber;
    public bool isEdge = true;
    

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
                        if (roomMask[locationx][locationz - 1] != -1 && this.locationz != roomMask[0].Length - 1)
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
