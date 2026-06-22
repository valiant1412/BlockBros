using UnityEngine;
using UnityEditor;
using System.IO; // Thư viện để can thiệp vào giao diện Unity

public class ClearSaveData
{
    // Dòng này tạo ra một menu mới trên cùng của Unity
    [MenuItem("Tools/Xóa toàn bộ PlayerPrefs")]
    public static void ClearAllPrefs()
    {
        // 1. Tìm đường dẫn đến két sắt
        string saveFilePath = Application.persistentDataPath + "/DinoBrosSave.json";


        // 2. Kiểm tra xem két sắt có tồn tại không
        if (File.Exists(saveFilePath))
        {
            // 3. Xóa sổ!
            File.Delete(saveFilePath);
            Debug.Log("🗑️ ĐÃ XÓA THÀNH CÔNG: Toàn bộ dữ liệu JSON đã bị xóa sạch! Game sẽ reset về Level 1.");
        }
        else
        {
            Debug.Log("⚠️ KHÔNG TÌM THẤY: File Save chưa được tạo hoặc đã bị xóa từ trước rồi.");
        }

        // Dọn dẹp luôn cả PlayerPrefs cũ để phòng hờ (nếu trước đó còn vương vãi)
        PlayerPrefs.DeleteAll();
    }
}