using UnityEditor;
using UnityEngine;


public class Map : Plain
{
    //offset for tile GameObject Placement
    public int tileOffsetX;
    public int tileOffsetY;

   

    //public GameObject[] ObjRefs;//may want to use more than tile and room
    //public GameObject tileRef;
    public GameObject roomRef;

    public int roomCount;//count of all room except the boss room

    //to generate the sizes of the 9 levels
    public int maxRoomSize, minRoomSize;

    [SerializeField]
    int TileCount = 0;//not really needed.

    GameObject[] rooms;

    void Awake()
    {

        buildPlain();//build location array for map
        

        //instantiate rooms, and set room sizes //Building mask
        rooms = new GameObject[roomCount];//allocate rooms
        for (int i = 0; i < roomCount; i++)
        {
            rooms[i] = Instantiate(roomRef.gameObject as GameObject) as GameObject;
            rooms[i].transform.parent = transform;//set at origin

            TileCount += rooms[i].GetComponent<Room>().floorCount = Random.Range(minRoomSize, maxRoomSize + 1);//set floor count for room
            
            rooms[i].GetComponent<Room>().init();//build room



            //place room mask
           // placeRoomMask(rooms[i].GetComponent<Room>());
        }

    }

    public void placeRoomMask(Room room)//not working
    {
        int rngx = Random.Range(0, width - room.coordsObjIdentifier.Length);
        int rngz = Random.Range(0, height - room.coordsObjIdentifier[0].Length);

        //check for overlap
        for (int i = rngz; i < height - room.coordsObjIdentifier[0].Length; i++)
        {
            for (int j = rngx; j < width - room.coordsObjIdentifier.Length; j++)
            {
                coordsObjIdentifier[j][i] = room.coordsObjIdentifier[j][i];
            }
        }

        for (int i = rngz; i < height - room.coordsObjIdentifier[0].Length; i++)
        {
            for(int j = rngx; j < width - room.coordsObjIdentifier.Length; j++)
            {
                coordsObjIdentifier[j][i] = room.coordsObjIdentifier[j][i];
            }
        }

        


        //howto place room
        int trys = 0;
        bool roomPlace = false;
        while (trys < 5 || !roomPlace)
        {

            trys++;
        }
        if (!roomPlace)
        {
            trys = 0;
            //extend map size
        }

    }


    public int deactivateRoomByIndex = -1;
    int activeRoom = 0; 

    public void Update()
    {

        if (deactivateRoomByIndex >= 0 && deactivateRoomByIndex < roomCount)
        {
            setroomActive(deactivateRoomByIndex);
            deactivateRoomByIndex = -1;
        }

    }
    public void setroomActive(int roomIndex)
    {
        bool isRoomActive = rooms[roomIndex].gameObject.activeInHierarchy;
        if (isRoomActive)
        rooms[deactivateRoomByIndex].SetActive(false);
        else
        {
            rooms[deactivateRoomByIndex].SetActive(true);
        }
    }

}
