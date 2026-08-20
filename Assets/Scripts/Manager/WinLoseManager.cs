using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;
    [Header("Nhân vật chơi")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Winzone")]

    [SerializeField] private GameObject winUI;

    [SerializeField] private GameObject loseUI;

    public bool isGameEnded = false;
    public bool isGameWon = false;
    private readonly HashSet<Player> exitedPlayers = new HashSet<Player>();
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        isGameEnded = false;
        isGameWon = false;

    }
    public void ResetResult()
    {
        isGameEnded = false;
        isGameWon = false;
        exitedPlayers.Clear();
    }

    public void NotifyPlayerExited(Player player)
    {
        if (isGameEnded || player == null || !exitedPlayers.Add(player)) return;

        player.SetState(PlayerState.Exit);
        player.gameObject.SetActive(false);

        if (exitedPlayers.Count == 2)
        {
            Win();
        }
    }
    public void Lose()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        if (AudioManager.instance != null) AudioManager.instance.PlayLose();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameplayActive(false);
            GameManager.Instance.RespawnPlayers();
        }

        Time.timeScale = 0f;

        loseUI.SetActive(true);
    }
    public void Win()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        isGameWon = true; // 2. THÊM DÒNG NÀY ĐỂ BÁO LÀ ĐÃ THẮNG

        if (GameManager.Instance != null) GameManager.Instance.SetGameplayActive(false);
        if (AudioManager.instance != null) AudioManager.instance.PlayWin();
        Time.timeScale = 0f;

        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
        var highestLevel = SaveManager.Instance.gameData.HighestLevel;
        // if (player1.currentState == PlayerState.Exit)
        // {
        //     player1.SetState(PlayerState.Stand);
        // }
        // if (player2.currentState == PlayerState.Exit)
        // {
        //     player2.SetState(PlayerState.Stand);
        // }
        // Chỉ cập nhật HighestLevel (Mở khóa màn mới)
        int nextLevel = currentLevel + 1;
        if (currentLevel >= highestLevel &&
            (LevelManager.Instance == null || LevelManager.Instance.HasLevel(nextLevel)))
        {
            SaveManager.Instance.gameData.HighestLevel = nextLevel;
            SaveManager.Instance.SaveGame();
        }

        winUI.SetActive(true);
        if (EconomyManager.Instance != null) EconomyManager.Instance.AddMoney(100);
    }

    public void SetActiveToWinUIToFalse()
    {
        if (winUI.activeInHierarchy)
        {
            winUI.SetActive(false);
        }
    }

    public void SetActiveToLoseUIToFalse()
    {
        if (loseUI.activeInHierarchy)
        {
            loseUI.SetActive(false);
        }
    }

}
