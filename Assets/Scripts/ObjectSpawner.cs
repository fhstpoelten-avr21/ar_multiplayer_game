using System.Collections;
using System.Collections.Generic;
using AR;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    private PlacementIndicator placementIndicator;
    private ARPlaneManager planeManager;

    void Start ()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();
        planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void Update ()
    {
        if((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) ||  Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(objectToSpawn, 
            placementIndicator.transform.position, placementIndicator.transform.rotation);
        }
    }
}