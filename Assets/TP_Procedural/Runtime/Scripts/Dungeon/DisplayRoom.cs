using System.Collections.Generic;
using UnityEngine;

public class DisplayRoom : MonoBehaviour {
    public int tailleX;
    public int tailleY;
    public int cell;
    public bool debug;
    List<GameObject> allRoomInDungeons = new List<GameObject>();
    public List<RoomSettings> roomPrefabs = new List<RoomSettings>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) {
            Display(DungeonGenerator.Instance.dungeonMap, tailleX, tailleY);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            DeleteDisplay();

        }
    }

    public void Display() {
        Display(DungeonGenerator.Instance.dungeonMap, tailleX, tailleY);
    }
    
    private void Display(List<Node> room, float sizeRoomX, float sizeRoomY)
    {
        foreach (Node node in room)
        {
            Vector2 positionRoom = new Vector2(node.PosX * sizeRoomX, node.PosY * sizeRoomY);
            GameObject a = Instantiate(GetRoom(node), positionRoom, Quaternion.identity);
            allRoomInDungeons.Add(a);
            SetDoor(node, a);
        }
    }

    public void DeleteDisplay()
    {
        for (int i = 0; i < allRoomInDungeons.Count; i++)
        {
            Destroy(allRoomInDungeons[i]);
        }
    }

    private GameObject GetRoom(Node node)
    {
        List<GameObject> result = new List<GameObject>();
        GameObject a;
        foreach (RoomSettings roomS in roomPrefabs)
        {
            a = null;
            switch (roomS.RoomTags)
            {
                case RoomType.RoomWithKey:
                    a = CheckIsGoodRoom(node, roomS, RoomTag.HasKey);                    
                    break;
                case RoomType.RoomStart:
                    a = CheckIsGoodRoom(node, roomS, RoomTag.IsSpawn);
                    break;
                case RoomType.RoomEnd:
                    a = CheckIsGoodRoom(node, roomS, RoomTag.IsExit);
                    break;
                case RoomType.RoomClassic:
                    bool b = node.RoomTags.HasFlag(RoomTag.IsSpawn);
                    if(!node.RoomTags.HasFlag(RoomTag.IsSpawn) && !node.RoomTags.HasFlag(RoomTag.IsExit) && !node.RoomTags.HasFlag(RoomTag.HasKey))
                        a = roomS.Room;
                    break;
                default:
                    break;
            }

            if (a != null)
                result.Add(a);
        }
        if(result.Count > 0)
        {
            return result[UnityEngine.Random.Range(0, result.Count - 1 )];
        }
        else
        return result[0];
    }

    private GameObject CheckIsGoodRoom(Node node, RoomSettings roomS, RoomTag tag)
    {
        if (node.RoomTags.HasFlag(tag))
            return roomS.Room;
        else
            return null;
    }


    private void SetDoor(Node node, Door door, RoomTag tagSelect, Door.STATE etatDoor)
    {
        if(node.RoomTags.HasFlag(tagSelect))            
            {
                door.SetState(etatDoor);
            }
    }

    private void SetDoor(Node node, GameObject room)
    {
        Door[] doorArray = room.GetComponentsInChildren<Door>();
        for (int i = 0; i < doorArray.Length; i++)
        {
            doorArray[i].SetState(Door.STATE.WALL);

            switch (doorArray[i].Orientation)
            {
                case Utils.ORIENTATION.NONE:
                    break;
                case Utils.ORIENTATION.NORTH:
                    SetDoor(node, doorArray[i], RoomTag.HasTopDoor, Door.STATE.OPEN);
                    SetDoor(node, doorArray[i], RoomTag.TopDoorLocked, Door.STATE.CLOSED);
                    break;
                case Utils.ORIENTATION.EAST:
                    SetDoor(node, doorArray[i], RoomTag.HasRightDoor, Door.STATE.OPEN);
                    SetDoor(node, doorArray[i], RoomTag.RightDoorLocked, Door.STATE.CLOSED);
                    break;
                case Utils.ORIENTATION.SOUTH:
                    SetDoor(node, doorArray[i], RoomTag.HasBottomDoor, Door.STATE.OPEN);
                    SetDoor(node, doorArray[i], RoomTag.BottomDoorLocked, Door.STATE.CLOSED);
                    break;
                case Utils.ORIENTATION.WEST:
                    SetDoor(node, doorArray[i], RoomTag.HasLeftDoor, Door.STATE.OPEN);
                    SetDoor(node, doorArray[i], RoomTag.LeftDoorLocked, Door.STATE.CLOSED);
                    break;
                default:
                    break;
            }
        }


    }

    private void OnDrawGizmos()
    {
        if(debug)
        {
            Display(DungeonGenerator.Instance.dungeonMap, tailleX, tailleY);
        }
    }
}
