using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour {
    public static DungeonGenerator Instance;

    public bool applicationQuit;
    
    [Header("-- Generation Parameters --")]
    
    [Range(1, 10)] public int lockedDoors;
    public int distanceMinBetweenStartAndEnd = 5;
    public int maxIteration = 4;

    public GameObject prefabSpawn;
    public GameObject prefabBranch;
    public GameObject prefabKey;
    public GameObject prefabExit;
    public GameObject prefabRoom;
    public float tileLength;
    public float offsetBetweenSprite;

    public Text textTries;
    public List<Node> dungeonMap;

    private readonly List<Node> waitingRoom = new List<Node>();

    private Node _spawnNode;
    private bool _isNewGeneration;
    private int generationIteration;

    private void Awake() {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        dungeonMap = new List<Node>();
        
        if (LevelParameters.Instance.seed == 0)
            NewSeed();

        LevelParameters.Instance.NextLevel();

        while (!GenerateDungeon()) {
            ClearDungeon();
            Debug.LogError("Cannot generate a good dungeon layout !");
            if (applicationQuit)
                Application.Quit(-1);
        }
    }

    private void Update() {
        if (_isNewGeneration)
            DisplayDungeon();
    }

    public void ResetDungeon() {
        ClearDungeon();
        GenerateDungeon();
    }

    private void ClearDungeon() {
        NewSeed();
        generationIteration = 0;
        dungeonMap.Clear();
        waitingRoom.Clear();
        while (transform.childCount != 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
    
    private void DisplayDungeon() {
        textTries.text = $"Essaies : {generationIteration}\n" +
                         $"Level Size : {LevelParameters.Instance.levelSize}\n" +
                         $"Branches : {LevelParameters.Instance.levelBranches}\n" +
                         $"Branches Length : {LevelParameters.Instance.levelBranchLength}\n";

        foreach (Node node in dungeonMap) {
            GameObject toGenerate;
            if (node.RoomTags.HasFlag(RoomTag.IsSpawn))
                toGenerate = prefabSpawn;
            else if (node.isBranch && !node.RoomTags.HasFlag(RoomTag.HasKey))
                toGenerate = prefabBranch;
            else if (node.RoomTags.HasFlag(RoomTag.HasKey))
                toGenerate = prefabKey;
            else if (node.RoomTags.HasFlag(RoomTag.IsExit))
                toGenerate = prefabExit;
            else
                toGenerate = prefabRoom;

            Vector3 position = new Vector3(
                node.PosX * (tileLength + offsetBetweenSprite),
                node.PosY * (tileLength + offsetBetweenSprite),
                0);
            
            Instantiate(toGenerate, position, Quaternion.identity, transform);

            if (!node.RoomTags.HasFlag(RoomTag.IsSpawn)) continue;
            
            position.z = Camera.main.transform.position.z;
            Camera.main.transform.position = position;
        }
        _isNewGeneration = false;
    }

    private void NewSeed() {
        Random.InitState(DateTime.Now.Millisecond);
        LevelParameters.Instance.seed = Random.Range(100000, 1000000);
    }

    private bool GenerateDungeon() {

        _isNewGeneration = true;
        Random.InitState(LevelParameters.Instance.seed);
        
        bool isDefined = true;
        Node lastNode = GenerateSpawn();
        
        lastNode = GeneratePath(ref lastNode, LevelParameters.Instance.levelSize, LevelParameters.Instance.levelBranches);

        if (lastNode == null)
            isDefined = false;

        return isDefined;
    }

    private Node GeneratePath(ref Node startNode, int length, int branches) {
        Node lastNode = startNode;
        List<Node> pathNode = new List<Node>();

        for (int i = 0; i < length; i++) {
            lastNode = DrawNode(lastNode, 0);

            if (lastNode == null) return null;

            pathNode.Add(lastNode);
            if (!waitingRoom.Contains(lastNode))
                waitingRoom.Add(lastNode);
        }
        
        lastNode.SetLocked();
        lastNode.AddFlag(RoomTag.IsExit);

        Debug.Log($"Waiting room {waitingRoom.Count}");
        
        if (!GenerateBranches(pathNode, branches)) return null;
        
        dungeonMap = dungeonMap.Union(waitingRoom).ToList();
        waitingRoom.Clear();

        return lastNode;
    }

    private bool GenerateBranches(List<Node> pathNodes, int branches) {
        List<Node> drawPool = pathNodes;
        bool keySet = false;

        drawPool = drawPool.Where(node => !node.RoomTags.HasFlag(RoomTag.IsExit)).ToList();
        int count = Mathf.Min(branches, drawPool.Count);
        for (int i = 0; i <= count; i++) {
            int index = Random.Range(0, drawPool.Count);
            bool hasKey = !keySet && (i > count / 2 || Random.Range(0, 11) > 5);

            if (hasKey)
                keySet = true;
            
            if (!GenerateBranch(drawPool[index], hasKey, 0)) return false;

            drawPool.RemoveAt(index);
        }
        
        return true;
    }
    
    private bool GenerateBranch(Node startNode, bool hasKey, int iteration) {
        int length = LevelParameters.Instance.levelBranchLength;
        Node lastNode = startNode;
        
        if (iteration > maxIteration) return false;
        
        for (int i = 0; i < length; i++) {
            lastNode = DrawNode(lastNode, 0);

            if (lastNode == null)
                return GenerateBranch(startNode, hasKey, iteration + 1);

            lastNode.isBranch = true;
            if (!waitingRoom.Contains(lastNode))
                waitingRoom.Add(lastNode);
        }
        
        if (hasKey) lastNode.AddFlag(RoomTag.HasKey);

        return true;
    }

    private Node DrawNode(Node lastNode, int iteration) {
        if (iteration > maxIteration) return null;
        
        List<Vector2> neighbors = new List<Vector2>();
        neighbors.AddRange(GetNeighbors(lastNode));

        if (neighbors.Count == 0) return DrawNode(lastNode, iteration + 1);
            
        Vector2 selectedNeighbor = neighbors[Random.Range(0, neighbors.Count)];

        return CreateNode(selectedNeighbor, ref lastNode);
    }

    private Node GetNode(int x, int y) {
        IEnumerable<Node> select = dungeonMap
            .Where(node => node.PosX == x && node.PosY == y);
        IEnumerable<Node> selectWaiting = waitingRoom
            .Where(node => node.PosX == x && node.PosY == y);

        select = select.Concat(selectWaiting);
        
        IEnumerable<Node> enumerable = select.ToList();
        return enumerable.ToList().Count > 0 ? enumerable.ElementAt(0) : null;
    }

    private List<Vector2> GetNeighbors(Node node) {
        List<Vector2> resultList = new List<Vector2>();

        // Case au dessus
        if (GetNode(node.PosX, node.PosY + 1) == null)
            resultList.Add(new Vector2(node.PosX, node.PosY + 1));

        // Case en dessous
        if (GetNode(node.PosX, node.PosY - 1) == null)
            resultList.Add(new Vector2(node.PosX, node.PosY - 1));
        
        // Case à droite
        if (GetNode(node.PosX + 1, node.PosY) == null)
            resultList.Add(new Vector2(node.PosX + 1, node.PosY));

        // Case à gauche
        if (GetNode(node.PosX - 1, node.PosY) == null)
            resultList.Add(new Vector2(node.PosX - 1, node.PosY));

        return resultList;
    }
    
    private Node GenerateSpawn() {
        Node spawn = new Node {
            PosX = Random.Range(-10, 10),
            PosY = Random.Range(-10, 10)
        };
        spawn.AddFlag(RoomTag.IsSpawn);
        _spawnNode = spawn;
        dungeonMap.Add(spawn);

        return spawn;
    }

    private Node CreateNode(Vector2 position, ref Node lastNode) {
        Node room = new Node {
            PosX = (int) position.x,
            PosY = (int) position.y
        };

        if (room.PosX == lastNode.PosX && room.PosY + 1 == lastNode.PosY) {
            room.AddFlag(RoomTag.HasTopDoor);
            lastNode.AddFlag(RoomTag.HasBottomDoor);
        } else if (room.PosX == lastNode.PosX && room.PosY - 1 == lastNode.PosY) {
            room.AddFlag(RoomTag.HasBottomDoor);
            lastNode.AddFlag(RoomTag.HasTopDoor);
        } else if (room.PosX + 1 == lastNode.PosX && room.PosY == lastNode.PosY) {
            room.AddFlag(RoomTag.HasRightDoor);
            lastNode.AddFlag(RoomTag.HasLeftDoor);
        } else if (room.PosX + 1 == lastNode.PosX && room.PosY == lastNode.PosY) {
            room.AddFlag(RoomTag.HasLeftDoor);
            lastNode.AddFlag(RoomTag.HasRightDoor);
        }
        
        return room;
    }
}
