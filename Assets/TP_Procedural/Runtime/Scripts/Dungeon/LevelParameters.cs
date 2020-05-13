using System.ComponentModel;
using UnityEngine;

public class LevelParameters : MonoBehaviour {
    public static LevelParameters Instance;

    [ReadOnly(true)] public int seed;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}