using UnityEditor;
using UnityEngine;

//generate room mask
public class Room : Plane
{
   

    public bool isBossRoom = false;
    public GameObject tileRef;
    public GameObject wallRef;

    //[SerializeField]
    //public Tile[] tileObjArr;

    public GameObject[] tileArr;
    public int[] externalEdges;
    public int floorCount;//set when Map instantiates room //get tile count
    [SerializeField]
    int edgeCount = 0;//starting with count of first tile
    [SerializeField]
    int tilesPlaced = -1;

    public int tileOffsetX;
    public int tileOffsetY;

    // Start is called before the first frame update
    public void init()//call me please
    {

        //set offset
        tileOffsetX = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().tileOffsetX;
        tileOffsetY = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().tileOffsetY;

       // tileObjArr = new Tile[floorCount];//allocate tiles based on tile count
        //instantiate the floor tiles for the room
        tileArr = new GameObject[floorCount];//removing tiles later
        externalEdges = new int[floorCount];//worst case n edge tiles

        //construct each of the floor tiles
        for(int i = 0; i < floorCount; i++)
        {
            tileArr[i] = Instantiate(tileRef.gameObject as GameObject) as GameObject;
            tileArr[i].transform.parent = transform;//set in middle location of room
        }

        

        BuildRoomMask();
        AutoResizePlane();

    }

    public void BuildRoomMask()
    {
        //generating room mask
        width = floorCount;
        height = width;
        buildPlane();//allocate room mask arr

        //internally set tile 0s location room center
        tileArr[0].GetComponent<Tile>().locationx = floorCount / 2;
        tileArr[0].GetComponent<Tile>().locationz = floorCount / 2;
        coordsObjIdentifier[floorCount / 2][floorCount / 2] = 0;//floor at center is 0

        tileArr[0].gameObject.GetComponent<Tile>().checkNeighbors(coordsObjIdentifier);
        //remove tiles from edgelist
        //if is edge add to edgelist

        tileArr[tilesPlaced + 1].transform.localPosition = new Vector3(tileArr[tilesPlaced + 1].GetComponent<Tile>().locationx * tileOffsetX, transform.parent.localPosition.y, tileArr[tilesPlaced + 1].GetComponent<Tile>().locationz * tileOffsetY);


        //checkAdjMask(tileArr[0].GetComponent<Tile>());
        //check for any false's, decide how to recount edges
        //UpdateEdges();
        tilesPlaced++;
        externalEdges[0] = 0;//0th tile is only edge tile
        

        

        while (tilesPlaced < floorCount -1)
        {
            //pick random number from edgelist
            int nextPlacementTile = Random.Range(0, edgeCount);
            //place tile at location
            //tileArr[nextPlacementTile] = 
            int nextPlacement = Random.Range(0, 4 - tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().neighborCount);
            //convert nextPlacement to actual
            int emptyFoundCount = -1;
            for(int i = 0; i < 4; i++)
            {
                if (tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().adjList[i] == -1)
                {
                    emptyFoundCount++;
                }
                if(emptyFoundCount == nextPlacement)
                {
                    nextPlacement = i;
                    i = 4;
                }
            }


            tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().tileNumber = tilesPlaced + 1;
            switch (nextPlacement)
            {
                case 0://north
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationx = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationx;
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationz = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationz - 1;

                    break;
                case 1://east
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationx = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationx + 1;
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationz = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationz;
                    break;
                case 2://south
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationx = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationx;
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationz = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationz +1;
                    break;
                default://west
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationx = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationx - 1;
                    tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationz = tileArr[nextPlacementTile].gameObject.GetComponent<Tile>().locationz;
                    break;
            }
            //add placement to mask
            coordsObjIdentifier[tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationx][tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().locationz] = tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().tileNumber;
            //check neighbors
            tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().checkNeighbors(coordsObjIdentifier);

            //get neighbors to update themselves
            for(int j = 0; j < 4; j++)
            {
                if (tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().adjList[j] > -1)
                {
                    tileArr[tileArr[tilesPlaced + 1].gameObject.GetComponent<Tile>().adjList[j]].gameObject.GetComponent<Tile>().checkNeighbors(coordsObjIdentifier);
                }
            }


            //place tile in world
            tileArr[tilesPlaced + 1].transform.localPosition = new Vector3( tileArr[tilesPlaced + 1].GetComponent<Tile>().locationx * tileOffsetX, transform.parent.localPosition.y,  tileArr[tilesPlaced + 1].GetComponent<Tile>().locationz * tileOffsetY);

            tilesPlaced++;

            //update edgelist
            edgeCount = 0;
            for(int i = 0; i < tilesPlaced + 1; i++)
            {
                if (tileArr[i].gameObject.GetComponent<Tile>().isEdge)
                {
                    externalEdges[edgeCount] = i;
                    edgeCount++;
                }
            }
           // tilesPlaced++;
        }
        
    }

    
    

    public void placeTiles()
    {
        //check how many tiles have been placed
    }


    public void activate()
    {
        for(int i = 0; i < floorCount; i++)
        {
            tileArr[i].SetActive(true);
        }
    }

    

}
