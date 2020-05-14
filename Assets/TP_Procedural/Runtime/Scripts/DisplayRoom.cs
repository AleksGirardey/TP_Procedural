using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DisplayRoom : MonoBehaviour
{
    public int tailleX;
    public int tailleY;
    public int cell;
    public bool debug;
    List<GameObject> allRoomInDungeons = new List<GameObject>();
    public List<RoomSettings> roomPrefabs = new List<RoomSettings>();

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Display(DungeonGenerator.Instance.dungeonMap, tailleX, tailleY);
 
        }
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


    private GameObject GetRoom(Node node)
    {
        GameObject result;
        foreach (RoomSettings roomS in roomPrefabs)
        {
            Debug.Log(node.RoomTags.HasFlag(roomS.RoomTags));
            if (roomS.RoomTags.HasFlag(node.RoomTags))
            {
                result = roomS.Room;
                return result;
            }
        }
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
