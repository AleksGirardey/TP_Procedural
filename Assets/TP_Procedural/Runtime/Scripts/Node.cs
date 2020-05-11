using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private enum TYPE
    {
        START,
        END,
        DOOR,
        KEY
    }
    private TYPE nodeType = TYPE.START;

    private int _difficulty = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
