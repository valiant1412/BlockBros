using UnityEngine;
using UnityEditor;
using System.IO; // Thư viện để can thiệp vào giao diện Unity

public class AddMoney
{
    // Dòng này tạo ra một menu mới trên cùng của Unity
    [MenuItem("Tools/Thêm tiền")]
    public static void Add()
    {
        // 1. Tìm đường dẫn đến két sắt
        var money = SaveManager.Instance.gameData.totalGold;
        money += 1000;
        SaveManager.Instance.gameData.totalGold = money;

        SaveManager.Instance.SaveGame();

    }
}