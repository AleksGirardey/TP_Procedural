using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRoom : MonoBehaviour
{
    public int tailleX;
    public int tailleY;
    public int cell;
    List<GameObject> allRoomInDungeons = new List<GameObject>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(DungeonGenerator.Instance.dungeonMap != null)
            {

            }
        }
    }

    private void Display(List<Node> room, float sizeRoomX, float sizeRoomY)
    {
        foreach (Node node in room)
        {
            Vector2 positionRoom = new Vector2(node.PosX * sizeRoomX, node.PosY * sizeRoomY);
            //allRoomInDungeons.Add(Instantiate())
            Gizmos.DrawCube((Vector3)positionRoom, new Vector3(cell,cell,0));
        }
    }

    private GameObject CreateRoom()
    {
        GameObject a = null;
        return a;
    }


    private void OnDrawGizmos()
    {
        if(DungeonGenerator.Instance.dungeonMap != null)
        {
            Display(DungeonGenerator.Instance.dungeonMap, tailleX, tailleY);
        }
    }
}
