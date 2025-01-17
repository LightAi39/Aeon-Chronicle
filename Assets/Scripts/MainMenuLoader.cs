using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    public async void LoadScene(string sceneToLoad)
    {
        Debug.Log("test");
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    private IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        Debug.Log("test");
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!loadOperation.isDone)
        {
            Debug.Log("loop");
            Debug.Log(Mathf.Clamp01(loadOperation.progress / 0.9f));
            yield return null;  
        }
    }
}
