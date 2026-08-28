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
    private const int FirstPlayableLevel = 1;

    [Header("Kho chứa toàn bộ giao diện")]
    public List<UIData> uiDatabase;

    [Header("Nút chỉ dùng trong gameplay")]
    // ButtonGroup con của Stop-Popup (1); chỉ bật sau khi người chơi bắt đầu một màn.
    [SerializeField] private GameObject stopPopupGameplayButtonGroup;

    // Chỉ true khi popup được mở trong gameplay; tránh bật input nhầm ở HomePage hoặc Shop.
    private bool resumeGameplayAfterClosingPanel;

    private void Start()
    {
        // Scene mở ở HomePage nên luôn ẩn các nút chỉ dành cho gameplay lúc khởi tạo.
        SetStopPopupGameplayButtons(false);
    }

    //  sửa khi mở setting hoặc pause
    public void OpenPanel(string targetName)
    {
        resumeGameplayAfterClosingPanel = false;

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

        // Mở HomePage từ bất kỳ luồng nào cũng phải trả Stop-Popup về trạng thái menu.
        if (targetName == "HomePage") SetStopPopupGameplayButtons(false);

        if (targetName == "Pause" || targetName == "Setting" || targetName == "Win" || targetName == "Lose" || targetName == "Unlock" || targetName == "NotEnoughCoin")
        {
            // Chỉ các popup Pause/Setting mở từ màn đang chơi mới cần trả input khi đóng.
            resumeGameplayAfterClosingPanel = (targetName == "Pause" || targetName == "Setting") &&
                                             GameManager.Instance != null &&
                                             GameManager.Instance.IsGameplayActive;
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
        FinishClosingPanel();
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        FinishClosingPanel();
    }

    private void FinishClosingPanel()
    {
        Time.timeScale = 1f; // Trả lại thời gian thực

        // Đóng Pause/Setting bằng nút Close cũng phải mở lại swipe, không chỉ riêng ResumeBtn.
        if (resumeGameplayAfterClosingPanel &&
            (WinLoseManager.Instance == null || !WinLoseManager.Instance.isGameEnded) &&
            GameManager.Instance != null)
        {
            GameManager.Instance.SetGameplayActive(true);
        }

        resumeGameplayAfterClosingPanel = false;
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
    }
    public void CloseShopPanel()
    {
        foreach (var data in uiDatabase)
        {
            if (data.panelObject == null) continue;

            if (data.panelName == "Shop") data.panelObject.SetActive(false);
            else if (data.panelName == "HomePage") data.panelObject.SetActive(true);
        }

        // Shop đóng về HomePage, vì vậy không để lại ButtonGroup của gameplay.
        SetStopPopupGameplayButtons(false);

        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
    }

    public void PlayBtn(GameObject homepage)
    {
        ClosePanel();
        homepage.SetActive(false);
        // Chỉ hiển thị ButtonGroup khi người chơi thực sự bước vào gameplay.
        SetStopPopupGameplayButtons(true);
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

        var targetLevel = SaveManager.Instance.gameData.CurrentLevel;

        // Đi từ màn thắng về Home phải chuẩn bị sẵn màn kế tiếp. Ở màn cuối,
        // quy tắc loop đưa người chơi về Level1 thay vì giữ lại Level3 ở nền.
        if (WinLoseManager.Instance != null && WinLoseManager.Instance.isGameWon)
        {
            if (!TryGetNextLoopLevel(targetLevel, out targetLevel))
            {
                return;
            }
        }

        // 🧹 DỌN DẸP CHIẾN TRƯỜNG: Bắt buộc phải hạ cờ trước khi chơi lại!
        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.isGameWon = false;
            WinLoseManager.Instance.isGameEnded = false;
        }

        if (LevelManager.Instance == null || !LevelManager.Instance.TryStartLevel(targetLevel.ToString()))
        {
            return;
        }

        Time.timeScale = 1f;
        ClosePanel();

        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.ResetResult();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameplayActive(false);
        }
        // Khi về HomePage, ẩn nhóm nút vốn chỉ có ý nghĩa trong gameplay.
        SetStopPopupGameplayButtons(false);
        HomePage.SetActive(true);

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
    }
    public void LevelBtn()
    {
        AudioManager.instance.PlayClickSFX();
        HapticManager.LightTaptic();
        Time.timeScale = 1f;

        // Nếu đi ra từ bảng Win, chọn level kế tiếp; sau màn cuối quay về Level1.
        if (WinLoseManager.Instance != null && WinLoseManager.Instance.isGameWon)
        {
            var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
            if (TryGetNextLoopLevel(currentLevel, out int targetLevel))
            {
                SaveManager.Instance.gameData.CurrentLevel = targetLevel;
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

        // Màn chọn level thuộc menu, không hiển thị ButtonGroup của Stop-Popup (1).
        SetStopPopupGameplayButtons(false);
    }
    public void NextBtn()
    {
        HapticManager.LightTaptic();

        // Không chuyển màn nếu các manager hoặc dữ liệu lưu chưa sẵn sàng.
        if (SaveManager.Instance == null || SaveManager.Instance.gameData == null || LevelManager.Instance == null)
        {
            return;
        }

        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
        if (!TryGetNextLoopLevel(currentLevel, out int targetLevel))
        {
            return;
        }

        // TryStartLevel sẽ nạp map, reset kết quả Win/Lose, hồi sinh nhân vật,
        // và lưu CurrentLevel. Điều này biến Level3 -> Level1 thành một lượt chơi mới thật sự.
        if (!LevelManager.Instance.TryStartLevel(targetLevel.ToString()))
        {
            return;
        }

        Time.timeScale = 1f;
        ClosePanel();
    }

    private bool TryGetNextLoopLevel(int currentLevel, out int targetLevel)
    {
        targetLevel = FirstPlayableLevel;
        if (LevelManager.Instance == null) return false;

        int nextLevel = currentLevel + 1;
        if (LevelManager.Instance.HasLevel(nextLevel))
        {
            targetLevel = nextLevel;
            return true;
        }

        // Không còn map kế tiếp: bắt đầu vòng chơi mới từ Level1, không giữ Level3 làm màn hiện tại.
        return LevelManager.Instance.HasLevel(FirstPlayableLevel);
    }

    private void SetStopPopupGameplayButtons(bool isVisible)
    {
        // Bảo vệ trường hợp tham chiếu chưa được gán trong Inspector.
        if (stopPopupGameplayButtonGroup != null)
        {
            stopPopupGameplayButtonGroup.SetActive(isVisible);
        }
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
