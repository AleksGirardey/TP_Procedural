﻿using System;

public enum NodeDifficulty {
    Easy,
    Medium,
    Hard
}

[Flags] public enum RoomTag {
    HasTopDoor = 0,
    HasRightDoor = 1,
    HasBottomDoor = 2,
    HasLeftDoor = 4,
    HasKey = 8,
    TopDoorLocked = 16,
    RightDoorLocked = 32,
    BottomDoorLocked = 64,
    LeftDoorLocked = 128,
    IsSpawn = 256,
    IsExit = 512
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
