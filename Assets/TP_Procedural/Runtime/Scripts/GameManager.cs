using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [HideInInspector] public bool hasReachedEnd;
    public GameObject player;

    private void Awake() {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        SetPlayerPosition();
    }

    private void Update() {
        if (!hasReachedEnd) return;
        
        hasReachedEnd = false;
        DungeonGenerator.Instance.NextLevel();
        SetPlayerPosition();
    }

    private void SetPlayerPosition() {
        // Recuperer le point de spawn sur la map généré
    }

    public void Play() {
        SceneManager.LoadScene("NoEndScene");
    }

    public void Quit() {
        Application.Quit(-1);
    }
}
