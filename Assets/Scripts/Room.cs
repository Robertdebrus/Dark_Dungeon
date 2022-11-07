using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//generate room mask
public class Room : Plane
{
    public bool isBossRoom = false;
    public GameObject tileRef;
    public GameObject wallRef;

    //[SerializeField]
    //public Tile[] tileObjArr;

    public GameObject[] tileArr;
    public GameObject[] wallArr;
    public int[] externalEdges;
    public int floorCount;//set when Map instantiates room //get tile count
    
    [SerializeField]
    int edgeCount = 0;//starting with count of first tile
    
    int tilesPlaced = -1;

    int tileOffsetX;
    int tileOffsetY;
  
    public void init()//call me please
    {
        //set offset of the tiles given from the Map
        tileOffsetX = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().tileOffsetX;
        tileOffsetY = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().tileOffsetY;

        //instantiate the Tile Objects
        tileArr = new GameObject[floorCount];//removing tiles later
        //keep track of the external edges
        externalEdges = new int[floorCount];//worst case n edge tiles

        //construct each of the floor tiles
        ConstructTiles();
    
        BuildRoomMask();//build initial mask
        
        placeTilesInWorld();

    }
    void ConstructTiles()
    {
        for (int i = 0; i < floorCount; i++)
        {
            tileArr[i] = Instantiate(tileRef.gameObject as GameObject) as GameObject;
            tileArr[i].transform.parent = transform;//set in middle location of room
        }
    }

    public void BuildRoomMask()
    {
        //generating room mask
        width = height = floorCount;
        buildPlane();//allocate room mask arr

        //place the rest of the tiles
        while (tilesPlaced < floorCount -1)
        {
            PlaceTiles();
            tilesPlaced++;
        }

        AutoResizePlane();//cut mask to better fit actual placement
    }

    
    
    public void placeTilesInWorld()//not placing at center of room gameObject
    {
        //center checking
        Pair center;
        center.x = width / 2;
        center.y = height / 2;
        //check how many tiles have been placed
        for(int i = 0; i < floorCount; i++)
        {
            Tile theTile = tileArr[i].GetComponent<Tile>();
            GameObject theTileGObj = tileArr[i].gameObject;

            tileArr[i].gameObject.transform.position = new Vector3((theTile.locationx - center.x) * tileOffsetX , transform.parent.localPosition.y, (theTile.locationz - center.y) * tileOffsetY );
        }
        placeWalls();
    }


    

    void placeWalls()
    {
        makeWalls(getWallCount());
        //place them on edgeTiles
        int wallsPlaced = 0;
        
    }

    int getWallCount()
    {
        int count = 0;
        for(int i = 0; i < edgeCount; i++)
        {
            count += 4 - tileArr[externalEdges[i]].GetComponent<Tile>().neighborCount;
        }
        return count;
    }
    void makeWalls(int wallCount)
    {
        wallArr = new GameObject[wallCount];
        for(int i = 0; i < wallCount; i++)
        {
            wallArr[i] = Instantiate(wallRef.gameObject as GameObject) as GameObject;
            wallArr[i].transform.parent = transform; 
        }
    }

    void PlaceTiles()
    {
        if (tilesPlaced > -1)
        {
            PlaceNextTile();
        }
        else
        {
            placeFirstTile();
            
        }
    }

    //after each tile is placed if any neighbors are no longer edges, need to update the edgeList
    void UpdateEdges(int[] adjList)
    {

        for (short j = 0; j < 4; j++)
        {

            if (adjList[j] > -1)//if position in adj list is a tile then check 
            {
                Tile neighborTile = tileArr[adjList[j]].gameObject.GetComponent<Tile>();
                neighborTile.checkNeighbors(coordsObjIdentifier);

                if (!neighborTile.isEdge)
                    UpdateEdgesHelper(neighborTile.tileNumber);
            }
        }

    }
    void UpdateEdgesHelper(int tileNumber)
    {

        for (int i = tileNumber; i < edgeCount - 1; i++)
        {
            externalEdges[i] = externalEdges[i + 1];
        }
        edgeCount--;
    }
    void PlaceNextTile()//relys that PlaceFirstTile was called prior
    {
        //pick random number from edgelist
        int nextPlacementTile = Random.Range(0, edgeCount);
        
        //nextPlacementTile known to be edge, so no more than 3 neighbors
        int nextPlacement = Random.Range(0, 3 - tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().neighborCount);
        //convert nextPlacement to actual
        int emptyFoundCount = -1;
        int[] adjList = tileArr[externalEdges[nextPlacementTile]].gameObject.GetComponent<Tile>().adjList;


        Tile newTile = tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>();
        //Give the next tile its index
        newTile.tileNumber = tilesPlaced + 1;

        for (int i = 0; i < 4; i++)//the tile is known to be an edge, but the nextPlacement var isnt directly referencing the position
        {
            if (adjList[i] == -1)
            {
                emptyFoundCount++;
                if (emptyFoundCount == nextPlacement)//sometimes never happening so something wrong with logic here
                {
                    SetTileLocation(newTile, tileArr[externalEdges[nextPlacementTile]].gameObject.GetComponent<Tile>(), i);
                    i = 4;//leave loop
                }
                
            }
            
        }
       
        //add placement to mask
        coordsObjIdentifier[newTile.locationx][newTile.locationz] = newTile.tileNumber;
        //check neighbors
        newTile.checkNeighbors(coordsObjIdentifier);
        if (newTile.isEdge)
        {
            edgeCount++;
            externalEdges[edgeCount] = newTile.tileNumber ;
        }


        UpdateEdges(newTile.GetComponent<Tile>().adjList);//problem
    }

    void SetTileLocation(Tile tileNew, Tile tileOld, int nextPlacement)
    {
        Debug.Log("tile number: " + tileNew.tileNumber);
        switch (nextPlacement)
        {
            case 0://north
                tileNew.locationx = tileOld.locationx;
                tileNew.locationz = tileOld.locationz - 1;
                break;
            case 1://east
                tileNew.locationx = tileOld.locationx + 1;
                tileNew.locationz = tileOld.locationz;
                break;
            case 2://south
                tileNew.locationx = tileOld.locationx;
                tileNew.locationz = tileOld.locationz + 1;
                break;
            default://west
                tileNew.locationx = tileOld.locationx - 1;
                tileNew.locationz = tileOld.locationz;
                break;
        }
        Debug.Log("Tile Location x: " + tileNew.locationx + " z: " + tileNew.locationz);
    }

    void placeFirstTile()//place next relys on this being called first
    {
        //internally set tile 0s location room center
        tileArr[0].GetComponent<Tile>().locationx = floorCount / 2;//tell tile where its x Location is  
        tileArr[0].GetComponent<Tile>().locationz = floorCount / 2;//tell tile where its z location is
        coordsObjIdentifier[floorCount / 2][floorCount / 2] = 0;//floor at center is 0
        
        externalEdges[0] = 0;//0th tile is only edge tile
    }

    

    public void activate()
    {
        for (int i = 0; i < floorCount; i++)
        {
            tileArr[i].SetActive(true);
        }
    }
}
