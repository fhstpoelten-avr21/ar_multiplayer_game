using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    public GameObject car;
    private ARRaycastManager rayManager;
    private GameObject visual;
    private bool carIsSet;

    void Start ()
    {
        // get the components
        rayManager = FindObjectOfType<ARRaycastManager>();
        rayManager.gameObject.SetActive(false);
        visual = transform.GetChild(0).gameObject;
        car.SetActive(false);

        // hide the placement visual
        carIsSet = false;
        visual.SetActive(true);
        
    }

    void Update ()
    {

        if (!carIsSet)
        {
            // shoot a raycast from the center of the screen
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

            // if we hit an AR plane, update the position and rotation
            if (hits.Count > 0)
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;

                if (!visual.activeInHierarchy)
                {
                    car.SetActive (true);
                    visual.SetActive(true);

                    if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
                    {
                        GameObject obj = Instantiate(car,
                        transform.position, transform.rotation);
                        carIsSet = true;
                    }
                } else
                {
                    car.SetActive(false);
                }

            }
        }
    }
}