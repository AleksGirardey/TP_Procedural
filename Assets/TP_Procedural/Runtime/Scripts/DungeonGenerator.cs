using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour {
    [Header("-- Generation Parameters --")]
    [ReadOnly(true)] public int seed;
    [Range(3, 30)] public int dungeonSize;
    [Range(1, 10)] public int lockedDoors;
    //public bool equalSplit;
    //public int[] percentageBySegments;
    public int distanceMinBetweenStartAndEnd = 5;

    [HideInInspector] public List<Node> dungeonMap;

    private Node _spawnNode;
    private Node _exitNode;
    private bool _isNewGeneration;

    private void Awake() {
        if (seed == 0)
            NewSeed();
        // if (percentageBySegments.Length != lockedDoors) {
        //     equalSplit = true;
        // }
    }

    private void Start() {
        GenerateDungeon();
    }

    private void Update() {
        if (_isNewGeneration)
            DisplayDungeon();
    }

    private void DisplayDungeon() {

        _isNewGeneration = false;
    }

    private void NewSeed() {
        Random.InitState(DateTime.Now.Millisecond);
        seed = Random.Range(100000, 1000000);
    }
    
    private void GenerateDungeon() {
        Node spawn;
        Node exit;
        Node room;

        int roomBeforeKey;
        
        Random.InitState(seed);

        _isNewGeneration = true;

        spawn = InitSpawn();
        exit = InitEnd(spawn);

        //ToDo: Rework loop to add normal rooms and key room
        for (int index = 0; index < lockedDoors; index++) {
            room = CreateNode(index + (dungeonSize - 2) / lockedDoors, spawn, exit);
            room.Type = NodeType.Door;
        }
    } 

    public Node GetNode(int x, int y) {
        return dungeonMap
            .Where(node => node.PosX == x && node.PosY == y)
            .ElementAt(0);
    }

    private Node InitSpawn() {
        Node spawn = new Node {
            PosX = 0,
            PosY = 0,
            Type = NodeType.Start
        };

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
            PosY = spawn.PosY + Mathf.Max(distanceMinBetweenStartAndEnd, dungeonSize),
            Type = NodeType.End
        };

        _exitNode = end;
        dungeonMap.Add(end);
        
        return end;
    }
}
