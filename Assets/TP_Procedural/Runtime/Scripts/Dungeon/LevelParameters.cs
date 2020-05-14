using System.ComponentModel;
using UnityEngine;

public class LevelParameters : MonoBehaviour {
    public static LevelParameters Instance;

    public int currentLevel;
    
    [Range(0, 1)] public float sizeRatio = 0.3f;
    [Range(0, 1)] public float branchesRatio = 0.5f;
    [Range(0, 1)] public float branchLengthRatio = 0.2f;
    
    [ReadOnly(true)] public int seed;
    public float levelSize = 5;
    public float levelBranches = 2;
    public float levelBranchLength = 3;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel() {
        currentLevel++;

        if (currentLevel > 4) return;
        
        levelSize += levelSize * sizeRatio;
        levelBranches += levelBranches * branchesRatio;
        levelBranchLength += levelBranchLength * branchLengthRatio;
    }
}