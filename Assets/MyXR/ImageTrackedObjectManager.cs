using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackedObjectManager : MonoBehaviour
{
    [SerializeField]
    XRReferenceImageLibrary referenceImageLibrary;

    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedObjects;

    // Debug Log Object
    public TextMeshProUGUI debugText1;
    public TextMeshProUGUI debugText2;
    public TextMeshProUGUI debugText3;
    public TextMeshProUGUI debugText4;
    public TextMeshProUGUI debugText5;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        trackedImageManager.referenceLibrary = referenceImageLibrary;

        spawnedObjects = new Dictionary<string, GameObject>();

        foreach(GameObject obj in placeablePrefabs)
        {
            GameObject newObject = Instantiate(obj);
            newObject.name = obj.name;
            newObject.SetActive(false);

            spawnedObjects.Add(newObject.name, newObject); // ��ųʸ� ���
        }
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
        // ���ο� �̹����� �����Ǿ��� ��
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateSpawnObject(trackedImage);
            debugText2.text = "Image Tracked";
        }
        // ������ �̹����� ������Ʈ �Ǿ��� ��
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateSpawnObject(trackedImage);
            debugText2.text = "Image Updated";
        }
        // ������ �̹����� ������� ��
        foreach (var trackedImage in eventArgs.removed)
        {
            spawnedObjects[trackedImage.name].SetActive(false);
            debugText2.text = "Image Losted";
        }
    }

    private void UpdateSpawnObject(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;
        GameObject trackedObject = spawnedObjects[referenceImageName];

        debugText4.text = $"TrackingState.Tracking == {trackedImage.trackingState}";

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            trackedObject.transform.position = trackedImage.transform.position;
            trackedObject.transform.rotation = trackedImage.transform.rotation;
            trackedObject.SetActive(true);
        }
        else
        {
            trackedObject.SetActive(false);
        }
    }

    private void Update()
    {
        Debug.Log($"There are {trackedImageManager.trackables.count} images being tracked");

        bool found = false;

        foreach(var trackedImage in trackedImageManager.trackables)
        {
            Debug.Log($"Images: {trackedImage.referenceImage.name} is at " + 
                $"{trackedImage.transform.position}");
            debugText1.text = $"Images: {trackedImage.referenceImage.name} is at " + 
                $"{trackedImage.transform.position}" + '\n';
            found = true;
            debugText3.text = "Trackables != null";
        }

        if (!found)
        {
            debugText3.text = "Trackables == null";
        }
    }
}
