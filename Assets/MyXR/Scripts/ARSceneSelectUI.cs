using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class ARSceneSelectUI : MonoBehaviour
{
    private bool isLoadingScene = false;

    public void LoadScene(string sceneName)
    {
        if (!isLoadingScene)
        {
            isLoadingScene = true;
            ResetImageTracking();
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 비동기 로드가 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 로드가 완료되면 다음 동작 실행
        DoNextAction();
    }

    private void DoNextAction()
    {
        // 씬 로드가 완료된 후에 실행할 동작을 여기에 작성
        Debug.Log("씬 로드 완료!");
    }

    // 이미지 추적을 초기화하는 메서드
    public  void ResetImageTracking()
    {
        ARTrackedImageManager trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = false;
            foreach (var trackedImage in trackedImageManager.trackables)
            {
                trackedImage.gameObject.SetActive(false);
            }
            trackedImageManager.enabled = true;
        }
    }
}
