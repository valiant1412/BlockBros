using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManagement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject stopPopup;

    [SerializeField] private AudioSource popUpSound;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RestartBtn()
    {
        var currentLevel = PlayerPrefs.GetInt("LevelInPlay");
        AudioManager.instance.PlayClickSFX();

        Time.timeScale = 1f;
        LevelManagement.Instance.StartLevel(currentLevel.ToString());
        stopPopup.SetActive(false);
    }
    public void StartBtn()
    {
        var currentLevel = PlayerPrefs.GetInt("LevelInPlay");
        AudioManager.instance.PlayClickSFX();

        if (currentLevel == 0)
        {
            PlayerPrefs.SetInt("LevelInPlay", 1);
            SceneManager.LoadScene((currentLevel + 1).ToString());
        }
        else
        {
            SceneManager.LoadScene((currentLevel).ToString());
        }
    }
    public void PauseBtn()
    {
        if (stopPopup == null) return;
        stopPopup.SetActive(true);

        popUpSound.Play();

        Time.timeScale = 0f;

    }
    public void ResumeBtn()
    {

        Debug.Log("Nút Resume ĐÃ ĐƯỢC BẤM!"); // Thêm dòng này
        if (stopPopup == null) return;
        AudioManager.instance.PlayClickSFX();

        Time.timeScale = 1f;
        stopPopup.SetActive(false);

    }
    public void HomeBtn()
    {
        SceneManager.LoadScene("StartScene");
        AudioManager.instance.PlayClickSFX();

        Time.timeScale = 1f;
    }
    public void LevelBtn()
    {
        AudioManager.instance.PlayClickSFX();
        // enable lại bảng
        stopPopup.SetActive(false);

        // 2. Báo cho Giám đốc dọn dẹp Map và mở Menu
        if (LevelManagement.Instance != null)
        {
            LevelManagement.Instance.ReturnToMenu();
        }
        Time.timeScale = 1f;
    }
    public void NextBtn()
    {
        var currentLevel = PlayerPrefs.GetInt("LevelInPlay");
        LevelManagement.Instance.StartLevel(currentLevel.ToString());
        AudioManager.instance.PlayClickSFX();
        Time.timeScale = 1f;
    }
}
