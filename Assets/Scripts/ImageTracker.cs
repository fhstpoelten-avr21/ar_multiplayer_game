using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToFollow;
    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
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
            UpdateObjectPose(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateObjectPose(trackedImage);
        }
    }

    private void UpdateObjectPose(ARTrackedImage trackedImage)
    {
        // Update the position and rotation of your GameObject
        objectToFollow.transform.position = trackedImage.transform.position;
        objectToFollow.transform.rotation = trackedImage.transform.rotation;
    }
}
