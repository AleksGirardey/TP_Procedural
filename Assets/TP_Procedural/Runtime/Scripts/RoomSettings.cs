using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSettings",menuName = "Settings/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    public GameObject Room;
    public List<RoomTag> Option = new List<RoomTag>();
}
