using UnityEngine;

public class FloorSpiked : MonoBehaviour {
    public GameObject spike;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        spike.SetActive(true);
        _collider.enabled = false;
    }
}
