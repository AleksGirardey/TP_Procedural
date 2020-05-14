using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [HideInInspector] public bool hasReachedEnd;
    public GameObject playerPrefab;

    private GameObject _playerInstance;
    
    public bool isPlayerDead;

    [HideInInspector] public GameObject spawnRoom;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (isPlayerDead) {
            isPlayerDead = false;
            Reload();
        }
        if (!hasReachedEnd) return;
        
        hasReachedEnd = false;
        DungeonGenerator.Instance.NextLevel();
    }

    public void Play() {
        SceneManager.LoadScene("NoEndScene");
    }

    public void Reload()
    {
        DungeonGenerator.Instance.ResetDungeon();
    }

    public void Quit() {
        Application.Quit(-1);
    }
}
