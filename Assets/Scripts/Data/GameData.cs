using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int HighestLevel;
    public int CurrentLevel;

    public bool isMuted;
    public bool isSFXOff;

    public bool isHapticOff;

    // Hàm khởi tạo dữ liệu mặc định cho người chơi mới
    public GameData()
    {
        HighestLevel = 1;
        CurrentLevel = 1;
        isMuted = false;
        isSFXOff = false;
        isHapticOff = false; // BẮT BUỘC PHẢI THÊM DÒNG NÀY
    }
}
