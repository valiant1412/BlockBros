using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    // Start is called before the first frame update
    public GameData gameData;

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
            string json = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(json);

            Debug.Log("📂 Đã tải dữ liệu thành công! Level cao nhất: " + gameData.HighestLevel);
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
        string json = JsonUtility.ToJson(gameData, true);

        // 2. Ghi đè chuỗi văn bản đó vào ổ cứng điện thoại
        File.WriteAllText(saveFilePath, json);

        Debug.Log("💾 Đã lưu game thành công tại: " + saveFilePath);
    }

}
