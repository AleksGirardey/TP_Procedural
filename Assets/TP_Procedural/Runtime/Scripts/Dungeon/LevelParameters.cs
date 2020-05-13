using System.ComponentModel;
using UnityEngine;

public class LevelParameters : MonoBehaviour {
    public static LevelParameters Instance;

    public int currentLevel;
    
    [Range(0, 1)] public float sizeRatio = 0.3f;
    [Range(0, 1)] public float branchesRatio = 0.5f;
    [Range(0, 1)] public float branchLengthRatio = 0.2f;
    
    [ReadOnly(true)] public int seed;
    public int levelSize = 5;
    public int levelBranches = 2;
    public int levelBranchLength = 3;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel() {
        currentLevel++;

        levelSize += Mathf.RoundToInt(levelSize * sizeRatio);
        levelBranches += Mathf.RoundToInt(levelBranches * branchesRatio);
        levelBranchLength += Mathf.RoundToInt(levelBranchLength * branchLengthRatio);
    }
}