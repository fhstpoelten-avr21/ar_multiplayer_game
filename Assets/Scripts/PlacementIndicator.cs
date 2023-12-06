using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    private ARRaycastManager rayManager;
    private ARPlaneManager planeManager;
    private XROrigin xrOrigin;

    // cars and field objects
    public GameObject[] team1Cars;
    public GameObject[] team2Cars;
    public GameObject field;

    // teams config
    private GameObject[] carsPlayer1Instances = { };
    private GameObject[] carsPlayer2Instances = { };

    // field config
    private bool fieldIsSet;
    private Vector3 originalFieldScale;
    private GameObject fieldInstance;

    public float fieldScale = 1.0f;
    private float _fieldScale = 1.0f;

    //public float fieldScale = 1f;
    public float carsScale = .08f;
    private float _carsScale = .08f;

    void Start()
    {
        // get the components
        rayManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        xrOrigin = FindObjectOfType<XROrigin>();

        instantiateField();
        instantiateTeams();
    }

    void Update()
    {
        if (!fieldIsSet)
        {
            // shoot a raycast from the center of the screen
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

            // if we hit an AR plane, update the position and rotation
            if (hits.Count > 0)
            {
                setHitPosition(hits[0]);
                fieldInstance.SetActive(true);

                if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
                {
                    setField();
                }

            }
            else
            {
                fieldInstance.SetActive(false);
            }
        }
    }

    private void OnValidate()
    {
        if(_fieldScale != fieldScale)
        {
            _fieldScale = fieldScale;
            updateFieldSize();
        }

        if(_carsScale != carsScale)
        {
            _carsScale = carsScale;
        }
    }

    // init method for creating player-cars and setting them on the fieldInstance
    private void instantiateTeams()
    {
        var fieldCollider = fieldInstance.GetComponent<MeshRenderer>();
        Vector3 fieldSize = fieldCollider.bounds.size;
        float fieldLength = Mathf.Max(fieldSize.x, fieldSize.z);
        float fieldWidth = Mathf.Min(fieldSize.x, fieldSize.z);
        float widthOffset = (fieldWidth / 5) /2;
        float lengthOffset = (fieldLength / 5) /2;

        int positionMultiplicator = 1;
        GameObject[][] teams = { team1Cars, team2Cars };
        GameObject[][] carInstances = { carsPlayer1Instances, carsPlayer2Instances };

        // loop through two teams
        for(var i = 1; i <= 2; i++)
        {
            // for each team, position the cars
            for(var j = 1; j <= teams[i-1].Length; j++)
            {
                GameObject carInstance = Instantiate(teams[i-1][j-1], new Vector3(0, 0), new Quaternion(0, 0, 0, 0));
                carInstance.transform.SetParent(fieldInstance.transform);
                carInstance.transform.localScale *= carsScale;

                float x = (fieldWidth / 5 * j) + ((fieldWidth / 5) / 2) + fieldWidth;
                float z = ((fieldLength / 5) + lengthOffset) * j;

                // position the cars one-fifth of the available team-space with an offset
                carInstance.transform.localPosition = new Vector3(x, 0, z*positionMultiplicator);
                carInstance.name = "car" + j + "Team" + i;
                carInstances[i-1].Append(carInstance);
            }

            positionMultiplicator *= -1;
        }
    }

    private void updateFieldSize()
    {
        fieldInstance.transform.localScale = originalFieldScale * _fieldScale;
    }

    private void instantiateField()
    {
        // init field to place
        fieldIsSet = false;
        fieldInstance = Instantiate(field, new Vector3(0, 0), new Quaternion(0, 0, 0, 0));

        // save the original size of the field
        originalFieldScale = fieldInstance.transform.localScale;

        fieldInstance.transform.localScale *= _fieldScale;
        fieldInstance.SetActive(false);
    }

    private void setHitPosition(ARRaycastHit hit)
    {
        fieldInstance.transform.position = hit.pose.position + new Vector3(0,.1f,0);
        fieldInstance.transform.rotation = hit.pose.rotation;
    }

    public void setVisibility(bool isVisible)
    {
        field.SetActive(isVisible);
    }

    // TODO: set field on current position and dont allow re-initialization
    void setField()
    {
        fieldIsSet = true;
    }
}