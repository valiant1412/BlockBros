using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable] // Bắt buộc có dòng này để nó hiện ra ngoài Inspector
public struct UIData
{
    public string panelName;      // Tên gọi nhớ (Ví dụ: "Pause", "Setting", "Shop", "Win")
    public GameObject panelObject; // Cục UI tương ứng kéo vào đây
}
public class ButtonManager : MonoBehaviour
{
    [Header("Kho chứa toàn bộ giao diện")]
    public List<UIData> uiDatabase;

    //  sửa khi mở setting hoặc pause
    public void OpenPanel(string targetName)
    {
        foreach (var data in uiDatabase)
        {
            if (data.panelName == targetName)
            {
                data.panelObject.SetActive(true);
            }
            else
            {
                data.panelObject.SetActive(false);
            }
        }
        if (targetName == "Pause" || targetName == "Setting" || targetName == "Win" || targetName == "Lose")
        {
            Time.timeScale = 0f;
            if (GameManager.Instance != null) GameManager.Instance.SetGameplayActive(false);
        }
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
    }

    public void ClosePanel()
    {
        foreach (var data in uiDatabase)
        {
            if (data.panelObject != null) data.panelObject.SetActive(false);
        }
        Time.timeScale = 1f; // Trả lại thời gian thực

        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
    }

    public void PlayBtn()
    {
        ClosePanel();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameplayActive(true);
        }
    }
    // Start is called before the first frame update
    // private bool isPause = false;

    public void RestartBtn()
    {
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();

        if (PlayerMoving.Instance != null) { PlayerMoving.Instance.ResetMovement(); }

        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;

        // 🧹 DỌN DẸP CHIẾN TRƯỜNG: Bắt buộc phải hạ cờ trước khi chơi lại!
        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.isGameWon = false;
            WinLoseManager.Instance.isGameEnded = false;
        }

        Time.timeScale = 1f;
        ClosePanel();
        LevelManager.Instance.StartLevel(currentLevel.ToString());

    }
    public void HomeBtn(GameObject HomePage)
    {
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();

        if (PlayerMoving.Instance != null) { PlayerMoving.Instance.ResetMovement(); }

        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;

        // 🧹 DỌN DẸP CHIẾN TRƯỜNG: Bắt buộc phải hạ cờ trước khi chơi lại!
        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.isGameWon = false;
            WinLoseManager.Instance.isGameEnded = false;
        }

        Time.timeScale = 1f;
        ClosePanel();
        LevelManager.Instance.StartLevel(currentLevel.ToString());
        HomePage.SetActive(true);
        GameManager.Instance.SetGameplayActive(true);
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
    // public void PauseBtn()
    // {
    //     if (isPause) return;
    //     if (Pop_up == null) return;
    //     Pop_up.SetActive(true);
    //     isPause = true;

    //     AudioManager.instance.PlayClickSFX();
    //     HapticManager.LightTaptic();

    //     Time.timeScale = 0f;

    // }
    public void ResumeBtn()
    {

        Debug.Log("Nút Resume ĐÃ ĐƯỢC BẤM!"); // Thêm dòng này

        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();

        Time.timeScale = 1f;
        ClosePanel();
        if (GameManager.Instance != null) GameManager.Instance.SetGameplayActive(true);
    }
    public void LevelBtn()
    {
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
        Time.timeScale = 1f;

        // Nếu đi ra từ bảng Win -> Cộng Level
        if (WinLoseManager.Instance != null && WinLoseManager.Instance.isGameWon)
        {
            var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
            var nextLevel = currentLevel + 1;
            if (LevelManager.Instance != null && LevelManager.Instance.HasLevel(nextLevel))
            {
                SaveManager.Instance.gameData.CurrentLevel = nextLevel;
                SaveManager.Instance.SaveGame();
            }
        }

        // 🧹 DỌN DẸP CHIẾN TRƯỜNG: Dù là thắng hay đang Pause thoát ra, ra Menu là phải hạ hết cờ!
        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.isGameWon = false;
            WinLoseManager.Instance.isGameEnded = false;
        }

        ClosePanel();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ReturnToMenu();
        }
    }
    public void NextBtn()
    {

        HapticManager.LightTaptic();

        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;

        if (LevelManager.Instance == null ||
            !LevelManager.Instance.TryStartLevel((currentLevel + 1).ToString())) return;

        Time.timeScale = 1f;
        ClosePanel();
    }
    // public void ExitPopUPBtn()
    // {
    //     Pop_up.SetActive(false);
    //     if (isPause) return;
    //     Time.timeScale = 1f;
    // }
    // public void SettingBtn()
    // {
    //     if (!isPause) isPause = true;
    //     if (Pop_up == null) return;
    //     Pop_up.SetActive(true);

    //     AudioManager.instance.PlayClickSFX();
    //     HapticManager.LightTaptic();
    //     if (isPause) return;
    //     Time.timeScale = 0f;
    // }
}
