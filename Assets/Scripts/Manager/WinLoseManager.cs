using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;
    private float playerArrived = 0;
    [Header("Nhân vật chơi")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Winzone")]
    [SerializeField] private Transform winzone1;
    [SerializeField] private Transform winzone2;

    [SerializeField] private GameObject winUI;

    [SerializeField] private GameObject loseUI;

    public bool isGameEnded = false;
    public bool isGameWon = false;
    void Start()
    {
        Instance = this;
        isGameEnded = false;
        isGameWon = false;

    }
    public bool CheckWinCondition()
    {
        Vector3 player1Position = new Vector3(player1.transform.position.x, 0f, player1.transform.position.z);
        Vector3 player2Position = new Vector3(player2.transform.position.x, 0f, player2.transform.position.z);

        // position of winzone
        Vector3 winzone1Position = new Vector3(winzone1.transform.position.x, 0f, winzone1.transform.position.z);
        Vector3 winzone2Position = new Vector3(winzone2.transform.position.x, 0f, winzone2.transform.position.z);

        //check distance
        bool p1_is_on_zone1 = Vector3.Distance(player1Position, winzone1Position) < 0.1f;
        bool p2_is_on_zone2 = Vector3.Distance(player2Position, winzone2Position) < 0.1f;

        bool p1_is_on_zone2 = Vector3.Distance(player1Position, winzone2Position) < 0.1f;
        bool p2_is_on_zone1 = Vector3.Distance(player2Position, winzone1Position) < 0.1f;

        if ((p1_is_on_zone1 && p2_is_on_zone2) || (p1_is_on_zone2 && p2_is_on_zone1))
        {
            return true;
        }
        return false;
    }
    public void Lose()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        gameObject.SetActive(true);
        AudioManager.instance.PlayLose();

        Time.timeScale = 0f;

        loseUI.SetActive(true);
    }
    public void Win()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        isGameWon = true; // 2. THÊM DÒNG NÀY ĐỂ BÁO LÀ ĐÃ THẮNG

        gameObject.SetActive(true);
        AudioManager.instance.PlayWin();
        Time.timeScale = 0f;

        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
        var highestLevel = SaveManager.Instance.gameData.HighestLevel;

        // Chỉ cập nhật HighestLevel (Mở khóa màn mới)
        if (currentLevel >= highestLevel)
        {
            SaveManager.Instance.gameData.HighestLevel = currentLevel + 1;
            SaveManager.Instance.SaveGame();
        }

        winUI.SetActive(true);
    }
    public void SetupInput(Player player1, Player player2, Transform winzone1, Transform winzone2)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.winzone1 = winzone1;
        this.winzone2 = winzone2;
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
