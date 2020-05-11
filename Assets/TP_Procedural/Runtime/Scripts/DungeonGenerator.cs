using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [Range(3, 30)]
    public int dungeonSize;

    public Node[][] _dungeonMap;
    
    private bool _isNewGeneration;

    private void Awake() {
        
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

    public void GenerateDungeon() {
        _isNewGeneration = true;

    }
    
}
