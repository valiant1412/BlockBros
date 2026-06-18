using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [Header("Nhân vật game")]
    [SerializeField] private GameObject mytext;

    [SerializeField] private Slider slider;

    [SerializeField] private GameObject btn;

    [SerializeField] private GameObject sceneTransitionObject;

    private bool isLoadDone = false;

    [Header("Tên Màn Chơi Chính")]

    private AsyncOperation asyncLoad;

    private int currentLevel;

    void Awake()
    {

    }
    void Start()
    {
        btn.SetActive(false);
        mytext.SetActive(true);
        slider.gameObject.SetActive(true);
        slider.value = 0;
        StartCoroutine(LoadGameInBackGround());


        //SceneTransition.Instance.SwitchLevel(currentLevel);

        asyncLoad.allowSceneActivation = true;
        SceneManager.LoadScene(currentLevel);
    }
    IEnumerator LoadGameInBackGround()
    {

        currentLevel = 1;

        asyncLoad = SceneManager.LoadSceneAsync(currentLevel);
        asyncLoad.allowSceneActivation = false;


        while (!asyncLoad.isDone)
        {
            // Nếu đã chạy xong 100% thanh giả lập VÀ hệ thống cũng đã load xong thật
            float progress = Mathf.Clamp01(asyncLoad.progress / .9f);
            slider.value = progress;
            if (asyncLoad.progress >= 0.9f)
            {
                // Tắt thanh bar và hiện nút Start
                slider.value = 1f;
                slider.gameObject.SetActive(false);
                mytext.SetActive(false);

            }
            yield return null;
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}
