using System.Collections.Generic;
using UnityEngine;

// 1. Nhét class bọc này ngay trên đầu file GameData.cs luôn
[System.Serializable]
public class PlayerInventory
{
    // Tủ đồ độc quyền của 1 nhân vật (Mặc định có sẵn skin gốc)
    // Tên ID skin gốc tôi đã sửa lại thành "default_skin" cho đồng bộ với mảng của bạn
    public List<string> OwnedSkins = new List<string> { "default_skin" };
}

[System.Serializable]
public class GameData
{
    [Header("Tiến trình chơi")]
    public int HighestLevel;
    public int CurrentLevel;

    [Header("Cài đặt hệ thống")]
    public bool isMuted;
    public bool isSFXOff;
    public bool isHapticOff;

    [Header("Kinh tế & Tủ đồ")]
    public int totalGold;

    // Mảng 2 phần tử tương ứng với Player 1 và Player 2
    public string[] currentSkin = new string[2] { "default_skin", "default_skin" };

    // ĐÃ CHUYỂN ĐỔI: Thay thế tủ đồ chung bằng mảng 2 tủ đồ riêng biệt cho 2 nhân vật
    public PlayerInventory[] Inventories;

    // Hàm khởi tạo dữ liệu mặc định cho người chơi mới tinh
    public GameData()
    {
        HighestLevel = 1;
        CurrentLevel = 1;
        isMuted = false;
        isSFXOff = false;
        isHapticOff = false;
        totalGold = 0;

        // ĐÒN QUYẾT ĐỊNH CHỐNG SẬP GAME: 
        // Ép máy tính phải cấp phát bộ nhớ cho 2 cái ngăn kéo ngay khi tạo tài khoản mới.
        Inventories = new PlayerInventory[2] {
            new PlayerInventory(), // Ngăn kéo của Player 1 (Index 0)
            new PlayerInventory()  // Ngăn kéo của Player 2 (Index 1)
        };
    }
}