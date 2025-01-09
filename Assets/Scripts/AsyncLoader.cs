using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] 
    private GameObject loadingScreen;
    [SerializeField] 
    private Animator _animator;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
    }
    public async void LoadScene(string sceneToLoad)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneSequence(sceneToLoad));
    }
    private IEnumerator LoadSceneSequence(string sceneToLoad)
    {
    yield return StartCoroutine(FadeInLoadingScreen());

    yield return StartCoroutine(LoadSceneAsync(sceneToLoad));
    }
    private IEnumerator FadeInLoadingScreen()
    {
        float fadeDuration = 0.4f;
        float elapsedTime = 0f;

        _canvasGroup.alpha = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1;
    }
    IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            Debug.Log(Mathf.Clamp01(loadOperation.progress / 0.9f));
            yield return null;  
        }
        
        yield return StartCoroutine(EngageAnimation());

        loadOperation.allowSceneActivation = true;
    }
    IEnumerator EngageAnimation()
    {
        if (_animator == null)
        {
            Debug.LogError("Animator not assigned!");
            yield break;
        }

        _animator.SetTrigger("LoadFinish");

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.IsInTransition(0))
        {
            yield return null;
        }
    }
}
