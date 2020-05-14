using System;

[Flags] public enum RoomTag {
    HasTopDoor = 1,
    HasRightDoor = 2,
    HasBottomDoor = 4,
    HasLeftDoor = 8,
    HasKey = 16,
    TopDoorLocked = 32,
    RightDoorLocked = 64,
    BottomDoorLocked = 128,
    LeftDoorLocked = 256,
    IsSpawn = 512,
    IsExit = 1024
}

[Flags]
public enum RoomType
{
    RoomWithKey = 1,
    RoomStart = 2,
    RoomEnd = 4,
    RoomClassic = 8,
}

public class Node 
{
    public RoomTag RoomTags;

    public bool IsBranch;
    
    public int PosX;
    public int PosY;

    public void AddFlag(RoomTag tag) {
        RoomTags |= tag;
    }

    public void RemoveFlag(RoomTag tag) {
        RoomTags &= ~tag;
    }

    public void SetFlag(RoomTag tag) {
        RoomTags = tag;
    }

    public void SetLocked(Node node) {
        if (node.RoomTags.HasFlag(RoomTag.TopDoorLocked)) AddFlag(RoomTag.BottomDoorLocked);
        if (node.RoomTags.HasFlag(RoomTag.RightDoorLocked)) AddFlag(RoomTag.LeftDoorLocked);
        if (node.RoomTags.HasFlag(RoomTag.BottomDoorLocked)) AddFlag(RoomTag.TopDoorLocked);
        if (node.RoomTags.HasFlag(RoomTag.LeftDoorLocked)) AddFlag(RoomTag.RightDoorLocked);
    }

    public void SetLocked() {
        if (RoomTags.HasFlag(RoomTag.HasTopDoor)) AddFlag(RoomTag.TopDoorLocked);
        if (RoomTags.HasFlag(RoomTag.HasRightDoor)) AddFlag(RoomTag.RightDoorLocked);
        if (RoomTags.HasFlag(RoomTag.HasBottomDoor)) AddFlag(RoomTag.BottomDoorLocked);
        if (RoomTags.HasFlag(RoomTag.HasLeftDoor)) AddFlag(RoomTag.LeftDoorLocked);
    }
}
