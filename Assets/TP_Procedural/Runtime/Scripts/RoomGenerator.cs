using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private List<GameObject> roomPrefabs = new List<GameObject>();

    private void Start()
    {
        
    }
    public void RoomInit(Node node)
    {
        //Récup les infos du node
    }

    private void GenerateRoom()
    {
        //Applique le prefab corrsepondant au type de room grace aux tags
    }

    private void GenerateDoors()
    {
        //Applique les portes grace aux tags
    }
}
