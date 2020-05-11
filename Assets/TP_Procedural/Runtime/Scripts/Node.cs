using UnityEngine;

public enum NodeType {
    Start,
    End,
    Door,
    Key
}

public enum NodeDifficulty {
    Easy,
    Medium,
    Hard
}

public class Node : MonoBehaviour {
    public int posX;
    public int posY;
    public NodeType type = NodeType.Start;
    public NodeDifficulty difficulty = 0;
    
}
