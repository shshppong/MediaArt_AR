using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class ARSceneSelectUI : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        ResetImageTracking();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // �̹��� ������ �ʱ�ȭ�ϴ� �޼���
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
