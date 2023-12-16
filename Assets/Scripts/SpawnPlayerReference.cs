using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerReference : MonoBehaviour
{
    private bool _hasPlayerReference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasPlayerReference()
    {
        return _hasPlayerReference;
    }

    public void SetPlayerReference(bool hasPlayerReference)
    {
        _hasPlayerReference = hasPlayerReference;
    }
}
