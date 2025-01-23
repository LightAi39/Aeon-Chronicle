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
    [SerializeField]
    private string animationToTrigger;
    private CanvasGroup _canvasGroup;
    private bool isLoading;

    private void Awake()
    {
        _canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
    }
    
    public async void LoadScene(string sceneToLoad)
    {
        if (isLoading) return; // Prevent duplicate loads
        isLoading = true;
        
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneSequence(sceneToLoad, animationToTrigger));
    }
    private IEnumerator LoadSceneSequence(string sceneToLoad, string animationToTrigger)
    {
        yield return StartCoroutine(FadeInLoadingScreen());

        yield return StartCoroutine(LoadSceneAsync(sceneToLoad, animationToTrigger));
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
    IEnumerator LoadSceneAsync(string sceneToLoad, string animationToTrigger)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            Debug.Log(Mathf.Clamp01(loadOperation.progress / 0.9f));
            yield return null;  
        }
        
        yield return StartCoroutine(EngageAnimation(animationToTrigger));

        loadOperation.allowSceneActivation = true;
        isLoading = false;
    }
    IEnumerator EngageAnimation(string animationToTrigger)
    {
        if (_animator == null)
        {
            Debug.LogError("Animator not assigned!");
            yield break;
        }

        _animator.SetTrigger(animationToTrigger);

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.IsInTransition(0))
        {
            yield return null;
        }
    }
}
