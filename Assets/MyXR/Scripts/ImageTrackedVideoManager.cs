using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Video;

public class ImageTrackedVideoManager : MonoBehaviour
{
    [SerializeField]
    XRReferenceImageLibrary referenceImageLibrary;

    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedObjects;

    // 비디오 플레이어 관련
    private List<VideoPlayer> videoPlayer;
    private List<RenderTexture> renderTexture;

    // 카운트
    int count = 0;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        trackedImageManager.referenceLibrary = referenceImageLibrary;

        spawnedObjects = new Dictionary<string, GameObject>();
        videoPlayer = new List<VideoPlayer>();
        renderTexture = new List<RenderTexture>();

    }

    private void Start()
    {
        count = 0;

        foreach (GameObject obj in placeablePrefabs)
        {
            // 이름으로 오브젝트 검색
            GameObject foundObject = GameObject.Find(obj.name);
            
            if (foundObject != null && foundObject.GetComponentInChildren<VideoPlayer>() != null)
            {
                videoPlayer.Add(foundObject.GetComponentInChildren<VideoPlayer>());
                renderTexture.Add(videoPlayer[count].targetTexture);
                
                // 오브젝트 활성화 하지 않고 로드하고 준비하는 코루틴
                StartCoroutine(PrepareVideo(videoPlayer[count]));

                spawnedObjects.Add(foundObject.name, foundObject);
                
                count++;
            }
        }
    }
    
    protected IEnumerator PrepareVideo(VideoPlayer source)
    {
        // 비디오 준비
        source.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!source.isPrepared)
        {
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("The video was load completed!");
        source.Play();
        //yield return new WaitForSeconds(0.01f);
        // 비디오가 준비되었으면 오브젝트를 활성화 시킨다.
        source.transform.parent.gameObject.SetActive(true);
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
        // 새로운 이미지가 감지되었을 때
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateSpawnObject(trackedImage);
        }
        // 감지된 이미지가 업데이트 되었을 때
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateSpawnObject(trackedImage);
        }
        // 감지된 이미지가 사라졌을 때
        foreach (var trackedImage in eventArgs.removed)
        {
            spawnedObjects[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateSpawnObject(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;
        GameObject trackedObject = spawnedObjects[referenceImageName];

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            trackedObject.transform.position = trackedImage.transform.position;
            trackedObject.transform.rotation = trackedImage.transform.rotation;
            trackedObject.SetActive(true);
            VideoPlayer video = trackedObject.GetComponentInChildren<VideoPlayer>();
            // 비디오 로드 될 때까지 대기
            StartCoroutine(PrepareVideo(video));
        }
        else
        {
            trackedObject.SetActive(false);
        }
    }

    private void Update()
    {
        Debug.Log($"There are {trackedImageManager.trackables.count} images being tracked");

        foreach(var trackedImage in trackedImageManager.trackables)
        {
            Debug.Log($"Images: {trackedImage.referenceImage.name} is at " + 
                $"{trackedImage.transform.position}");
        }
    }
}
