using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour {
    [Header("-- Generation Parameters --")]
    [ReadOnly(true)] public int seed;
    [Range(3, 30)] public int dungeonSize;
    [Range(1, 10)] public int lockedDoors;
    public int distanceMinBetweenStartAndEnd = 5;
    public int maxIteration = 4;

    public GameObject prefabSpawn;
    public GameObject prefabExit;
    public GameObject prefabRoom;
    public float tileLength;
    public float offsetBetweenSprite;

    public Text textTries;
    
    //[HideInInspector]
    public List<Node> dungeonMap;

    private Node _spawnNode;
    private Node _exitNode;
    private bool _isNewGeneration;
    private int generationIteration = 0;

    private void Awake() {
        if (seed == 0)
            NewSeed();
        // if (percentageBySegments.Length != lockedDoors) {
        //     equalSplit = true;
        // }
    }

    private void Start() {
        dungeonMap = new List<Node>();
        
        if (GenerateDungeon()) return;
        
        Debug.LogError("Cannot generate a good dungeon layout !");
        Application.Quit(-1);
    }

    private void Update() {
        if (_isNewGeneration)
            DisplayDungeon();
    }

    public void ResetDungeon() {
        NewSeed();
        generationIteration = 0;
        dungeonMap.Clear();
        while (transform.childCount != 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
        GenerateDungeon();
    }
    
    private void DisplayDungeon() {
        textTries.text = $"Essaies : {generationIteration}";
        GameObject toGenerate;
        foreach (Node node in dungeonMap) {
            if (node.RoomTags.HasFlag(RoomTag.IsSpawn))
                toGenerate = prefabSpawn;
            else if (node.RoomTags.HasFlag(RoomTag.IsExit))
                toGenerate = prefabExit;
            else
                toGenerate = prefabRoom;

            Instantiate(toGenerate,
                new Vector3(
                    node.PosX * (tileLength + offsetBetweenSprite),
                    node.PosY * (tileLength + offsetBetweenSprite),
                    0),
                Quaternion.identity,
                transform);
        }
        _isNewGeneration = false;
    }

    private void NewSeed() {
        Random.InitState(DateTime.Now.Millisecond);
        seed = Random.Range(100000, 1000000);
    }

    private bool GenerateDungeon() {
        generationIteration++;

        if (generationIteration > maxIteration) return false;
        
        Node lastNode;
        bool isDefined = true;
        
        Random.InitState(seed);
        _isNewGeneration = true;
        
        lastNode = InitSpawn();
        List<Vector2> neighbors = new List<Vector2>();
        Vector2 selectedNeighbor;
        
        for (int i = 0; i < dungeonSize; i++) {
            neighbors.Clear();
            neighbors.AddRange(GetNeighbors(lastNode));
            if (neighbors.Count == 0) {
                isDefined = false;
                lastNode = null;
                break;
            }

            selectedNeighbor = neighbors[Random.Range(0, neighbors.Count)];

            lastNode = new Node {
                PosX = (int) selectedNeighbor.x,
                PosY = (int) selectedNeighbor.y
            };

            dungeonMap.Add(lastNode);
        }
        
        lastNode?.AddFlag(RoomTag.IsExit);
        
        return isDefined || GenerateDungeon();
    } 

    public Node GetNode(int x, int y) {
        IEnumerable<Node> select = dungeonMap
            .Where(node => node.PosX == x && node.PosY == y);

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
    
    private Node InitSpawn() {
        Node spawn = new Node {
            PosX = 0,
            PosY = 0
        };
        spawn.AddFlag(RoomTag.IsSpawn);
        
        _spawnNode = spawn;
        dungeonMap.Add(spawn);

        return spawn;
    }

    private Node CreateNode(int index, Node spawn, Node end) {
        Node room = new Node {
            PosX = 0,
            PosY = spawn.PosY + index
        };

        dungeonMap.Add(room);
        
        return room;
    }
    
    private Node InitEnd(Node spawn) {
        Node end = new Node {
            PosX = spawn.PosX,
            PosY = spawn.PosY + Mathf.Max(distanceMinBetweenStartAndEnd, dungeonSize)
        };

        end.AddFlag(RoomTag.IsExit);
        _exitNode = end;
        dungeonMap.Add(end);
        
        return end;
    }
}
