using System;
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
    public GameObject ball;
    private GameObject ballInstance;

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
        instantiateBall();
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
        if (_fieldScale != fieldScale)
        {
            _fieldScale = fieldScale;
            updateFieldSize();
        }

        if (_carsScale != carsScale)
        {
            _carsScale = carsScale;
        }
    }

    // init method for creating player-cars and setting them on the fieldInstance
    private void instantiateTeams()
    {
        var fieldCollider = fieldInstance.GetComponent<MeshRenderer>();

        int positionMultiplicator = 1;
        GameObject[][] teams = { team1Cars, team2Cars };
        GameObject[][] carInstances = { carsPlayer1Instances, carsPlayer2Instances };

        // loop through two teams
        for (int i = 0; i < 2; i++)
        {
            int numberOfLanes = Convert.ToInt32(Math.Ceiling((double)teams[i].Length / 3));
            int numberOfPlayers = teams[i].Length;
            int currentPlayerIndex = 0;
            Vector3[][] slotGrid = calculateSlots(numberOfLanes, positionMultiplicator, fieldCollider, fieldInstance);
            carInstances[i] = new GameObject[numberOfPlayers];

            // instantiate Players
            for (int j = 0; j < numberOfPlayers; j++)
            {
                carInstances[i][j] = Instantiate(teams[i][j], new Vector3(0, 0), new Quaternion(0, 0, 0, 0));
                carInstances[i][j].transform.SetParent(fieldInstance.transform);
                carInstances[i][j].transform.localScale *= carsScale;
                carInstances[i][j].transform.LookAt(ballInstance.transform.position);
                carInstances[i][j].name = "car" + j + 1 + "Team" + i + 1;
            }

            // for each lane
            for (int j = 0; j < numberOfLanes; j++)
            {
                if (numberOfPlayers == 1)
                {
                    carInstances[i][currentPlayerIndex].transform.localPosition = slotGrid[j][1];
                    currentPlayerIndex++;
                }
                else if (numberOfPlayers == 2)
                {
                    carInstances[i][currentPlayerIndex].transform.localPosition = slotGrid[j][0];
                    carInstances[i][currentPlayerIndex + 1].transform.localPosition = slotGrid[j][2];
                    currentPlayerIndex += 2;
                }
                else
                {
                    carInstances[i][currentPlayerIndex].transform.localPosition = slotGrid[j][0];
                    carInstances[i][currentPlayerIndex + 1].transform.localPosition = slotGrid[j][1];
                    carInstances[i][currentPlayerIndex + 2].transform.localPosition = slotGrid[j][2];
                    currentPlayerIndex += 3;
                    numberOfPlayers -= 3;
                }
            }

            positionMultiplicator *= -1;
        }
    }

    void instantiateBall()
    {
        ballInstance = Instantiate(ball, new Vector3(0, ball.transform.right.x/2, 0), new Quaternion(0, 0, 0, 0));
        ballInstance.transform.SetParent(fieldInstance.transform);
    }

    private Vector3[][] calculateSlots(int numberOfLanes, int positionMultiplicator, MeshRenderer fieldMeshRenderer, GameObject fieldInstance)
    {
        Vector3 calculatedBounds = calculateBounds(fieldInstance.transform.localScale, fieldMeshRenderer.bounds.size);
        float fieldWidth = calculatedBounds.x;
        float fieldLength = calculatedBounds.z/2;
        Vector3[][] slotGrid = new Vector3[numberOfLanes][];

        for (int i = 0; i < numberOfLanes; i++)
        {
            slotGrid[i] = new Vector3[3];
            float lengthPos = fieldLength - (fieldLength / 4f) + ((((fieldLength / 4f) * 2f) / numberOfLanes) * -(i+i*0.5f));
            
            for (int j = 0; j < 3; j++)
            {
                Vector3 pos = new Vector3(0f, 0f, lengthPos * positionMultiplicator);
                slotGrid[i][j] = pos;
            }

            slotGrid[i][0].x = fieldWidth / 4f;
            slotGrid[i][2].x = -(fieldWidth / 4f);
        }

        return slotGrid;
    }

    Vector3 calculateBounds(Vector3 scale, Vector3 size)
    {
        // Element-wise division
        Vector3 result = new Vector3(size.x / scale.x, size.y / scale.y, size.z / scale.z);
        return result;
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
        fieldInstance.transform.position = hit.pose.position + new Vector3(0, .1f, 0);
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