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


    private void Display(List<Node> room, float sizeRoomX, float sizeRoomY)
    {
        foreach (Node node in room)
        {
            Vector2 positionRoom = new Vector2(node.PosX * sizeRoomX, node.PosY * sizeRoomY);
            //allRoomInDungeons.Add(Instantiate())
            Gizmos.DrawCube((Vector3)positionRoom, new Vector3(cell,cell,0));
        }
    }


    public GameObject GetRoom(Node node)
    {
        GameObject result;
        foreach (RoomSettings roomS in roomPrefabs)
        {
            if (node.RoomTags == roomS.RoomTags)
            {
                result = roomS.Room;
                return result;
            }
        }
        return null;
    }


    private void OnDrawGizmos()
    {
        if(debug)
        {
            Display(DungeonGenerator.Instance.dungeonMap, tailleX, tailleY);
        }
    }
}
