public enum NodeType {
    Start,
    End,
    Door,
    Key
}

public enum NodeDirection {
    Top,
    Right,
    Bottom,
    Left
}

public enum NodeDifficulty {
    Easy,
    Medium,
    Hard
}

public class Node {
    public int PosX;
    public int PosY;
    public NodeType          Type = NodeType.Start;
    public NodeDifficulty    Difficulty = 0;
    //public NodeDirection     NextNodeDirection = NodeDirection.Right;
    //public Node              NextNode;
    public Connection[]      Connections;
}
