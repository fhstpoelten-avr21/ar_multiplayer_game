using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class detection : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;

    public GameObject prefab;
    private float counter = 0;
    private int multiplicator = 1;

    //private void Start()
    //{
    //    // Call the function every 10 seconds, starting after 2 seconds.
    //    InvokeRepeating("YourFunction", 2f, 10f);

    //    // Stop the repetition after 30 seconds (for example).
    //    Invoke("StopRepeating", 30f);
    //}

    //private void move()
    //{

    //    // Your code here
    //    Debug.Log("Function called every 10 seconds");
    //}

    //private void StopRepeating()
    //{
    //    // Stop the repetition of YourFunction.
    //    CancelInvoke("YourFunction");

    //    Debug.Log("Repeating stopped");
    //}

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            prefab.transform.position = newImage.transform.position;
            print("DETECTED: " + newImage.referenceImage.name + newImage.referenceImage.size);
            // Handle added event
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            prefab.transform.position = updatedImage.transform.position;
            print("DETECTED UPDATE " + updatedImage.referenceImage.name + updatedImage.referenceImage.size);
            // Handle updated event
        }

        foreach (var removedImage in eventArgs.removed)
        {
            print("REMOVED");
            // Handle removed event
        }
    }
}
