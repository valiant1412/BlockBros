using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
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
        // 1. CHẶN ĐỨNG BÓNG MA: Báo cho Giám đốc di chuyển dọn dẹp Coroutine
        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.ResetMovement();
        }

        // 3. MỞ KHÓA THỜI GIAN: Lúc Game Over bạn đã set Time.timeScale = 0, 
        // giờ chơi lại bắt buộc phải trả về 1, nếu không game sẽ bị đơ!
        Time.timeScale = 1f;

        // 4. Sinh ra Map mới (Gọi lại hàm StartLevel hoặc logic sinh map của bạn)
        // Ví dụ: StartLevel(currentLevelIndex);

        // 5. Ẩn bảng Game Over đi
        // losePanel.SetActive(false);

        Time.timeScale = 1f;
        LevelManager.Instance.StartLevel(currentLevel.ToString());
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
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ReturnToMenu();
        }
        Time.timeScale = 1f;
    }
    public void NextBtn()
    {
        var currentLevel = PlayerPrefs.GetInt("LevelInPlay");
        LevelManager.Instance.StartLevel(currentLevel.ToString());

        AudioManager.instance.PlayClickSFX();
        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.SetActiveToWinUIToFalse();
        }

        Time.timeScale = 1f;
    }
}
