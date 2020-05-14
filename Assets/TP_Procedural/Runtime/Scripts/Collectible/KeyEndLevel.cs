using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEndLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent != Player.Instance.gameObject.transform)
            return;

        Debug.Log("Player triggered end key. Next lvl now !");
        DungeonGenerator.Instance.NextLevel();
    }
}
