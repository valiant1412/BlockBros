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
        SceneManager.LoadScene(currentLevel.ToString());
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
        SceneManager.LoadScene("LevelMenu");
        AudioManager.instance.PlayClickSFX();

        Time.timeScale = 1f;
    }
    public void NextBtn()
    {
        var currentLevel = PlayerPrefs.GetInt("LevelInPlay");
        SceneManager.LoadScene((currentLevel + 1).ToString());

        AudioManager.instance.PlayClickSFX();

        // lấy ra màn cao nhất đã đạt được
        var highestLevel = PlayerPrefs.GetInt("HighestLevel");
        if (currentLevel + 1 > highestLevel)
        {
            PlayerPrefs.SetInt("HighestLevel", currentLevel + 1);
        }
        PlayerPrefs.SetInt("LevelInPlay", currentLevel + 1);

        Time.timeScale = 1f;
    }
}
