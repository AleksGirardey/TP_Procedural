using System;

public enum NodeDifficulty {
    Easy,
    Medium,
    Hard
}

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

public class Node 
{
    public RoomTag RoomTags;

    public bool isBranch;
    
    public int PosX;
    public int PosY;
    public NodeDifficulty    Difficulty = 0;

    public void AddFlag(RoomTag tag) {
        RoomTags |= tag;
    }

    public void RemoveFlag(RoomTag tag) {
        RoomTags &= ~tag;
    }

    public void SetFlag(RoomTag tag) {
        RoomTags = tag;
    }

    public void SetLocked() {
        if (RoomTags.HasFlag(RoomTag.HasTopDoor)) AddFlag(RoomTag.TopDoorLocked);
        if (RoomTags.HasFlag(RoomTag.HasRightDoor)) AddFlag(RoomTag.RightDoorLocked);
        if (RoomTags.HasFlag(RoomTag.HasBottomDoor)) AddFlag(RoomTag.BottomDoorLocked);
        if (RoomTags.HasFlag(RoomTag.HasLeftDoor)) AddFlag(RoomTag.LeftDoorLocked);
    }
}
