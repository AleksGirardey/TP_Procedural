using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour {
    public static DungeonGenerator Instance;

    public bool applicationQuit;
    [Header("-- Generation Parameters --")]
    public int maxIteration = 4;

    private GameObject _playerInstance;
    public GameObject prefabPlayer;
    public List<Node> dungeonMap;

    private readonly List<Node> _waitingRoom = new List<Node>();
    
    private bool _isNewGeneration;
    private Coroutine _generationCoroutine;

    private CameraFollow _mainCameraFollow;

    [HideInInspector] public GameObject spawnRoom;

    private DisplayRoom _displayRoom;

    private void Awake() {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        _mainCameraFollow = Camera.main.GetComponent<CameraFollow>();
        _displayRoom = GetComponent<DisplayRoom>();
    }

    private void Start() {
        dungeonMap = new List<Node>();
        
        if (LevelParameters.Instance.seed == 0)
            NewSeed();

        LevelParameters.Instance.NextLevel();

        while (!GenerateDungeon()) {
            ClearDungeon();
            Debug.LogWarning("Cannot generate a good dungeon layout !");
            if (applicationQuit)
                Application.Quit(-1);
        }
    }

    private void Update() {
        if (_isNewGeneration)
            DisplayDungeon();
        if (applicationQuit) StopCoroutine(_generationCoroutine);
    }

    public void ResetDungeon() {
        ClearDungeon();

        _generationCoroutine = StartCoroutine(NewDungeonCoroutine());
    }

    private IEnumerator NewDungeonCoroutine() {
        int iteration = 0;
        while (!GenerateDungeon()) {
            ClearDungeon();
            iteration++;
        }
        
        Debug.LogWarning($"Iteration before generating a dungeon : {iteration}");

        yield return null;
    }

    public void NextLevel() {
        LevelParameters.Instance.NextLevel();
        ResetDungeon();
    }
    
    private void ClearDungeon() {
        NewSeed();
        //_generationIteration = 0;
        dungeonMap.Clear();
        _waitingRoom.Clear();
        _displayRoom.DeleteDisplay();
        if (_playerInstance) Destroy(_playerInstance);
        while (transform.childCount != 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    private void SetPlayerPosition() {
        if (_playerInstance) DestroyImmediate(_playerInstance);
        _playerInstance = Instantiate(prefabPlayer);

        Vector3 spawnRoomPosition = spawnRoom.transform.position;
        
        _playerInstance.transform.position = 
            new Vector3(
                spawnRoomPosition.x + 0.5f + _displayRoom.tailleX / 2f,
                spawnRoomPosition.y + 0.5f + _displayRoom.tailleY / 2f, 1);
        _mainCameraFollow.target = _playerInstance;

    }
    
    private void DisplayDungeon() {
        _displayRoom.Display();
        SetPlayerPosition();
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
        
        lastNode = GeneratePath(
            ref lastNode, 
            Mathf.RoundToInt(LevelParameters.Instance.levelSize),
            Mathf.RoundToInt(LevelParameters.Instance.levelBranches));

        if (lastNode == null)
            isDefined = false;

        return isDefined;
    }

    private Node GeneratePath(ref Node startNode, int length, int branches) {
        Node lastLastNode = null;
        Node lastNode = startNode;
        List<Node> pathNode = new List<Node>();

        for (int i = 0; i < length; i++) {
            lastLastNode = lastNode;
            lastNode = DrawNode(lastNode, false, 0);

            if (lastNode == null) return null;
            
            pathNode.Add(lastNode);
            if (!_waitingRoom.Contains(lastNode))
                _waitingRoom.Add(lastNode);
        }
        
        lastNode.SetLocked();
        lastLastNode?.SetLocked(lastNode);
        lastNode.AddFlag(RoomTag.IsExit);

        Debug.Log($"Waiting room {_waitingRoom.Count}");
        
        if (!GenerateBranches(pathNode, branches)) return null;
        
        dungeonMap = dungeonMap.Union(_waitingRoom).ToList();
        _waitingRoom.Clear();

        return lastNode;
    }

    private bool GenerateBranches(List<Node> pathNodes, int branches) {
        List<Node> drawPool = pathNodes;
        bool keySet = false;
        bool secretSet = false;

        drawPool = drawPool.Where(node => !node.RoomTags.HasFlag(RoomTag.IsExit)).ToList();
        int count = Mathf.Min(branches, drawPool.Count);
        for (int i = 0; i < count; i++) {
            int index = Random.Range(0, drawPool.Count);
            bool hasKey = !keySet && (i > count / 2 || Random.Range(0, 11) > 5);
            bool hasSecret = !secretSet && (i > count / 2 || Random.Range(0, 100) > 35);

            if (hasKey) keySet = true;
            if (hasSecret) secretSet = true;
            
            if (!GenerateBranch(drawPool[index], hasKey, hasSecret, 0)) return false;

            drawPool.RemoveAt(index);
        }
        
        return true;
    }
    
    private bool GenerateBranch(Node startNode, bool hasKey, bool hasSecret, int iteration) {
        int length = Mathf.RoundToInt(LevelParameters.Instance.levelBranchLength);
        Node lastNode = startNode;
        bool doesBreak = false;
        
        if (iteration > maxIteration) return false;
        
        for (int i = 0; i < length; i++) {
            Node lastLastNode = lastNode;
            lastNode = DrawNode(lastNode, false, 0);

            if (lastNode == null) {
                if (!GenerateBranch(startNode, hasKey, hasSecret, iteration + 1) && i < length / 2)
                    return false;
                doesBreak = true;
                lastNode = lastLastNode;
            }
            
            lastNode.IsBranch = true;
            if (!_waitingRoom.Contains(lastNode))
                _waitingRoom.Add(lastNode);

            if (doesBreak) break;
        }
        
        if (hasKey) lastNode.AddFlag(RoomTag.HasKey);
        
        if (!hasSecret) return true;
        
        lastNode = DrawNode(lastNode, true, 0);
        if (lastNode == null) return false;
        lastNode.AddFlag(RoomTag.IsHidden);
        _waitingRoom.Add(lastNode);
        return true;
    }

    private Node DrawNode(Node lastNode, bool secretRoom, int iteration) {
        if (iteration > maxIteration) return null;
        
        List<Vector2> neighbors = new List<Vector2>();
        neighbors.AddRange(GetNeighbors(lastNode));

        if (neighbors.Count == 0) return DrawNode(lastNode, secretRoom, iteration + 1);
            
        Vector2 selectedNeighbor = neighbors[Random.Range(0, neighbors.Count)];
        
        return CreateNode(selectedNeighbor, ref lastNode, secretRoom);
    }

    private Node GetNode(int x, int y) {
        IEnumerable<Node> select = dungeonMap
            .Where(node => node.PosX == x && node.PosY == y);
        IEnumerable<Node> selectWaiting = _waitingRoom
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
        dungeonMap.Add(spawn);

        return spawn;
    }

    private Node CreateNode(Vector2 position, ref Node lastNode, bool secretRoom) {
        Node room = new Node {
            PosX = (int) position.x,
            PosY = (int) position.y
        };

        if (room.PosX == lastNode.PosX && room.PosY + 1 == lastNode.PosY) {
            room.AddFlag(RoomTag.HasTopDoor);
            lastNode.AddFlag(RoomTag.HasBottomDoor);
            
            if (!secretRoom) return room;
            
            room.AddFlag(RoomTag.TopDoorHidden);
            lastNode.AddFlag(RoomTag.BottomDoorHidden);
        } else if (room.PosX == lastNode.PosX && room.PosY - 1 == lastNode.PosY) {
            room.AddFlag(RoomTag.HasBottomDoor);
            lastNode.AddFlag(RoomTag.HasTopDoor);

            if (!secretRoom) return room;
            
            room.AddFlag(RoomTag.BottomDoorHidden);
            lastNode.AddFlag(RoomTag.TopDoorHidden);
        } else if (room.PosX + 1 == lastNode.PosX && room.PosY == lastNode.PosY) {
            room.AddFlag(RoomTag.HasRightDoor);
            lastNode.AddFlag(RoomTag.HasLeftDoor);

            if (!secretRoom) return room;
            
            room.AddFlag(RoomTag.RightDoorHidden);
            lastNode.AddFlag(RoomTag.LeftDoorHidden);
        } else if (room.PosX - 1 == lastNode.PosX && room.PosY == lastNode.PosY) {
            room.AddFlag(RoomTag.HasLeftDoor);
            lastNode.AddFlag(RoomTag.HasRightDoor);

            if (!secretRoom) return room;
            
            room.AddFlag(RoomTag.LeftDoorHidden);
            lastNode.AddFlag(RoomTag.RightDoorHidden);
        }

        return room;
    }
}
