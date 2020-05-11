using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [Header("-- Generation Parameters --")]
    [Range(3, 30)] public int dungeonSize;
    [Range(1, 10)] public int lockedDoors;
    public bool equalSplit;
    public int[] percentageBySegments;
    
    [HideInInspector] public List<Node> dungeonMap;

    private Node _spawnNode;
    private bool _isNewGeneration;

    private void Awake() {
        if (percentageBySegments.Length != lockedDoors) {
            equalSplit = true;
        }
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

    private void GenerateDungeon() {
        _isNewGeneration = true;
        
    }

    public Node GetNode(int x, int y) {
        return dungeonMap
            .Where(node => node.posX == x && node.posY == y)
            .ElementAt(0);
    }
    
}
