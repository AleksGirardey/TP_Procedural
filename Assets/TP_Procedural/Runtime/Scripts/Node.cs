using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private enum TYPE
    {
        START,
        END,
        DOOR,
        KEY
    }
    private TYPE nodeType = TYPE.START;

    private enum DIRECTION
    {
        TOP,
        BOTTOM,
        RIGHT,
        LEFT
    }
    private DIRECTION nodeDirection = DIRECTION.RIGHT;

    private Node targetedNode = null;

    private bool linkedToTarget = false;
    
    private int difficulty = 0;

    private Connection[] connections;
}
