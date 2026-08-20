using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    // Start is called before the first frame update
    public GameData gameData;

    public static Action OnGoldChanged;
    private string saveFilePath;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject); return;
        }
        saveFilePath = Application.persistentDataPath + "/DinoBrosSave.json";
        LoadGame();
    }
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                gameData = JsonUtility.FromJson<GameData>(json);

                if (gameData == null) throw new InvalidDataException("Save data is empty or invalid.");
                if (gameData.RepairMissingData()) SaveGame();

                Debug.Log("📂 Đã tải dữ liệu thành công! Level cao nhất: " + gameData.HighestLevel);
            }
            catch (Exception exception)
            {
                Debug.LogError("Could not load save data. A new save will be created. " + exception.Message);
                gameData = new GameData();
                SaveGame();
            }
        }
        else
        {
            gameData = new GameData();
            SaveGame();
            Debug.Log("✨ Đã tạo file save mới cho người chơi lần đầu!");
        }
    }

    public void SaveGame()
    {
        if (gameData == null) gameData = new GameData();
        gameData.RepairMissingData();
        string json = JsonUtility.ToJson(gameData, true);

        // 2. Ghi đè chuỗi văn bản đó vào ổ cứng điện thoại
        File.WriteAllText(saveFilePath, json);

        Debug.Log("💾 Đã lưu game thành công tại: " + saveFilePath);
    }

}
