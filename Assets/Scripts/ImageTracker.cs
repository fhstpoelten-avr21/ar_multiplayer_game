using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject gameField;

    [SerializeField]
    private GameObject XROrigin;

    public GameObject uI_GameField_Btns;
    public GameObject uI_GameField_Remove_Btn;
    public GameObject uI_GameField_Set_Btn;
    public GameObject uI_GameField_Done_Btn;

    private ARTrackedImageManager trackedImageManager;
    private ARTrackedImage fieldTrackedImage;

    private string FIELD_QR = "QR_Code";
    private bool positioningAllowed = true;

    private void Awake()
    {
        trackedImageManager = XROrigin.GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if(trackedImage.referenceImage.name == FIELD_QR && positioningAllowed)
            {
                gameField.SetActive(true);
                fieldTrackedImage = trackedImage;
                UpdateObjectPose(trackedImage);
                ShowSetButton(true);
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name == FIELD_QR && positioningAllowed)
            {
                fieldTrackedImage = trackedImage;
                UpdateObjectPose(trackedImage);
                ShowSetButton(true);
            }
        }
    }

    private void UpdateObjectPose(ARTrackedImage trackedImage)
    {
        // Update the position and rotation of your GameObject
        gameField.transform.position = trackedImage.transform.position + Vector3.up * 0.1f;
        //objectToFollow.transform.rotation = trackedImage.transform.rotation;
    }

    public void SetGameField()
    {
        positioningAllowed = false;
        gameField.transform.position = fieldTrackedImage.transform.position + Vector3.up * 0.1f;
        uI_GameField_Done_Btn.SetActive(true);
        uI_GameField_Remove_Btn.SetActive(true);
        uI_GameField_Set_Btn.SetActive(false);
    }

    public void RemoveGameField()
    {
        positioningAllowed = true;
        uI_GameField_Remove_Btn.SetActive(false);
        uI_GameField_Done_Btn.SetActive(false);
        uI_GameField_Set_Btn.SetActive(true);
    }

    public void FinishPositioning()
    {
        GameManager.Instance.ShowSearchForPlayersInfoPanel();
    }

    public void Reposition()
    {
        positioningAllowed = true;
    }

    private void ShowSetButton(bool show)
    {
        uI_GameField_Set_Btn.SetActive(true);
    }
}
