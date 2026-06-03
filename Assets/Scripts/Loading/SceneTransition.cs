using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration;

    void Awake()
    {
        Instance = this;
        // Áp dụng tấm khiên bảo vệ Singleton chống rác bộ nhớ
        // if (Instance != null && Instance != this)
        // {
        //     Destroy(this.gameObject);
        // }
        // else
        // {
        //     Instance = this;
        //     // QUAN TRỌNG: Giữ cái rèm này sống xuyên qua các Scene
        //     DontDestroyOnLoad(this.gameObject);
        // }
    }
    private void Start()
    {
        // if (fadeCanvasGroup.alpha >= 0.9f)
        // {
        //     StartCoroutine(Fade(0));
        // }
    }
    public IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
        if (targetAlpha <= 0.1f)
        {
            fadeCanvasGroup.blocksRaycasts = false; // Mở lại tương tác
        }
    }
    public IEnumerator TransitionAndLoadLevel(int level)
    {
        yield return StartCoroutine(Fade(1));
        yield return null;


    }
    public void SwitchLevel(int level)
    {
        StartCoroutine(TransitionAndLoadLevel(level));
    }
}
