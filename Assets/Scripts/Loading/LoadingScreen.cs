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

    [Header("Tên Màn Chơi Chính")]

    private AsyncOperation asyncLoad;

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
    }
    IEnumerator LoadGameInBackGround()
    {   
        var currentLevel = PlayerPrefs.GetInt("LevelInPlay");
        if(currentLevel == 0)
        {
            PlayerPrefs.SetInt("LevelInPlay",1);
           
        }
        asyncLoad = SceneManager.LoadSceneAsync(currentLevel);
        asyncLoad.allowSceneActivation = false;


        while (!asyncLoad.isDone)
        {   
            // Nếu đã chạy xong 100% thanh giả lập VÀ hệ thống cũng đã load xong thật
            float progress = Mathf.Clamp01(asyncLoad.progress/.9f);
            slider.value = progress;
            if (asyncLoad.progress >= 0.9f)
            {
                // Tắt thanh bar và hiện nút Start
                slider.gameObject.SetActive(false);
                btn.SetActive(true);
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
