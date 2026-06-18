using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject Pop_up;
    private bool isPause = false;

    public void RestartBtn()
    {
        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
        // 1. CHẶN ĐỨNG BÓNG MA: Báo cho Giám đốc di chuyển dọn dẹp Coroutine
        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.ResetMovement();
        }

        Time.timeScale = 1f;

        Time.timeScale = 1f;
        LevelManager.Instance.StartLevel(currentLevel.ToString());
        Pop_up.SetActive(false);
        isPause = false;
    }
    public void StartBtn()
    {
        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;

        if (currentLevel == 0)
        {
            SaveManager.Instance.gameData.CurrentLevel = 1;
            SceneManager.LoadScene((currentLevel + 1).ToString());
        }
        else
        {
            SceneManager.LoadScene((currentLevel).ToString());
        }
    }
    public void PauseBtn()
    {
        if (isPause) return;
        if (Pop_up == null) return;
        Pop_up.SetActive(true);
        isPause = true;

        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();

        Time.timeScale = 0f;

    }
    public void ResumeBtn()
    {

        Debug.Log("Nút Resume ĐÃ ĐƯỢC BẤM!"); // Thêm dòng này
        if (Pop_up == null) return;
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();

        Time.timeScale = 1f;
        Pop_up.SetActive(false);
        isPause = false;
    }
    public void HomeBtn()
    {
        SceneManager.LoadScene("StartScene");
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();

        Time.timeScale = 1f;
    }
    public void LevelBtn()
    {
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
        // enable lại bảng
        Pop_up.SetActive(false);

        // 2. Báo cho Giám đốc dọn dẹp Map và mở Menu
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ReturnToMenu();
        }
        Time.timeScale = 1f;
        isPause = false;
    }
    public void NextBtn()
    {
        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
        LevelManager.Instance.StartLevel(currentLevel.ToString());

        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.SetActiveToWinUIToFalse();
        }

        Time.timeScale = 1f;
        isPause = false;
    }
    public void ExitPopUPBtn()
    {
        Pop_up.SetActive(false);
        if (isPause) return;
        Time.timeScale = 1f;
    }
    public void SettingBtn()
    {
        if (!isPause) isPause = true;
        if (Pop_up == null) return;
        Pop_up.SetActive(true);

        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
        if (isPause) return;
        Time.timeScale = 0f;
    }
}
