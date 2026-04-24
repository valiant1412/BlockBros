using UnityEngine;
using UnityEditor; // Thư viện để can thiệp vào giao diện Unity

public class ClearSaveData 
{
    // Dòng này tạo ra một menu mới trên cùng của Unity
    [MenuItem("Tools/Xóa toàn bộ PlayerPrefs")]
    public static void ClearAllPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Đã xóa sạch save game để test!");
    }
}