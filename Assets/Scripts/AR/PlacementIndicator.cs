using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR
{
    public class PlacementIndicator : MonoBehaviour
    {
        private ARRaycastManager rayManager;
        private ARPlaneManager planeManager;
        private XROrigin xrOrigin;

        [Header("Teams config")]
        [Space(1)]
        // cars and field objects
        public GameObject[] team1Cars;
        public GameObject[] team2Cars;

        // teams config
        private GameObject[] carsPlayer1Instances = { };
        private GameObject[] carsPlayer2Instances = { };
        private GameObject gameFieldContainer;

        // field config
        private bool fieldIsSet;
        private Vector3 originalFieldScale;
        private GameObject fieldInstance;
        private MeshRenderer fieldCollider;

        [Header("Field config")]
        [Space(1)]
        public GameObject field;
        public float fieldScale = 0.1f;
        private float _fieldScale = 0.1f;

        [Header("Car config")]
        [Tooltip("This controls the scale of the cars relative to the size of the field (e.g. 0.1 = 10% of fieldsize).")]
        [Space(1)]
        public float carsScale = 0.005f;
        private float _carsScale = 0.005f;

        [Header("Ball config")]
        [Space(1)]
        public GameObject ball;
        private GameObject ballInstance;
        private Ball ballScript;
        public float ballScale = 1f;
        private float _ballScale = 1f;

        void Start()
        {
            rayManager = FindObjectOfType<ARRaycastManager>();
            planeManager = FindObjectOfType<ARPlaneManager>();
            xrOrigin = FindObjectOfType<XROrigin>();

            gameFieldContainer = new GameObject("gameFieldContainer");
            gameFieldContainer.transform.position = Vector3.zero;
            gameFieldContainer.transform.rotation = new Quaternion(0, 0, 0, 0);
            gameFieldContainer.transform.localScale = Vector3.one;
        
            ballScript = ball.GetComponent<Ball>();
        
            instantiateField();
            instantiateBall();
            //instantiateTeams();
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

        // update sizes when changed in Unity UI Editor
        private void OnValidate()
        {
            if (_fieldScale != fieldScale || _carsScale != carsScale || _ballScale != ballScale)
            {
                _fieldScale = fieldScale;
                _carsScale = carsScale;
                _ballScale = ballScale;
                updateFieldSize();
            }
        }

        private void instantiateField()
        {
            // init field to place
            fieldIsSet = false;
            fieldInstance = Instantiate(field, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

            // save the original size of the field
            originalFieldScale = field.transform.localScale;

            fieldInstance.transform.SetParent(gameFieldContainer.transform);
            fieldInstance.transform.localScale = field.transform.localScale * _fieldScale;
            fieldInstance.SetActive(false);

            fieldCollider = fieldInstance.GetComponent<MeshRenderer>();
        }

        // init method for creating player-cars and setting them on the fieldInstance
        private void instantiateTeams()
        {
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
                    carInstances[i][j] = Instantiate(teams[i][j], new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    carInstances[i][j].transform.SetParent(gameFieldContainer.transform);
                    carInstances[i][j].transform.localScale = Vector3.one * _carsScale;

                    // Apply the rotation to the GameObject, preserving the existing X and Z rotations
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

                // Make the cars rotate to center of field
                numberOfPlayers = teams[i].Length;
                for (int j = 0; j < numberOfPlayers; j++)
                {
                    carInstances[i][j].transform.LookAt(fieldInstance.transform, Vector3.up);
                }

                positionMultiplicator *= -1;
            }

            carsPlayer1Instances = carInstances[0];
            carsPlayer2Instances = carInstances[1];
        }

        void instantiateBall()
        {
            ballInstance = Instantiate(ball, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            ballInstance.transform.SetParent(gameFieldContainer.transform);
            ballInstance.transform.localScale = ball.transform.localScale * _ballScale;
            MeshRenderer meshR = ballInstance.GetComponent<MeshRenderer>();
            ballInstance.transform.position = Vector3.zero + Vector3.up * meshR.bounds.size.y/2;
        }

        private Vector3[][] calculateSlots(int numberOfLanes, int positionMultiplicator, MeshRenderer fieldMeshRenderer, GameObject fieldInstance)
        {
            Vector3 calculatedBounds = fieldMeshRenderer.bounds.size;
            float fieldWidth = calculatedBounds.x;
            float fieldLength = calculatedBounds.z / 2;
            Vector3[][] slotGrid = new Vector3[numberOfLanes][];

            for (int i = 0; i < numberOfLanes; i++)
            {
                slotGrid[i] = new Vector3[3];
                float lengthPos = fieldLength - (fieldLength / 4f) + ((((fieldLength / 4f) * 2f) / numberOfLanes) * -(i + i * 0.5f));

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
            if (fieldInstance)
            {
                fieldInstance.transform.localScale = field.transform.localScale * _fieldScale;

                GameObject[][] teams = { team1Cars, team2Cars };
                GameObject[][] carsInstances = { carsPlayer1Instances, carsPlayer2Instances };

                for (int i = 0; i < carsInstances.Length; i++)
                {
                    for (int j = 0; j < carsInstances[i].Length; j++)
                    {
                        carsInstances[i][j].transform.localScale = teams[i][j].transform.localScale * _carsScale;
                    }
                }

                if (ballInstance)
                {
                    ballInstance.transform.localScale = ball.transform.localScale * _ballScale;
                }
            }
        }

        private void setHitPosition(ARRaycastHit hit)
        {
            gameFieldContainer.transform.position = hit.pose.position + new Vector3(0, .1f, 0);
            gameFieldContainer.transform.rotation = hit.pose.rotation;
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
}