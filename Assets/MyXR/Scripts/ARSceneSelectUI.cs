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

        // �񵿱� �ε尡 �Ϸ�� ������ ���
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // �ε尡 �Ϸ�Ǹ� ���� ���� ����
        DoNextAction();
    }

    private void DoNextAction()
    {
        // �� �ε尡 �Ϸ�� �Ŀ� ������ ������ ���⿡ �ۼ�
        Debug.Log("�� �ε� �Ϸ�!");
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
