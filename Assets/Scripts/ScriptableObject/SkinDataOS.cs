using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // BẮT BUỘC: Thần chú này giúp Unity vẽ class này ra ngoài Inspector
public class SkinItem
{
    public string skinID;

    public string skinName;
    public int price;
    public Material skinMaterial;
    public Sprite skinIcon;
}

// 2. TẠO KHUÔN ĐÚC (Kế thừa ScriptableObject)
[CreateAssetMenu(fileName = "SkinDatabase", menuName = "Shop/Skin Database")]
public class SkinDatabaseSO : ScriptableObject
{
    [Header("Kho chứa toàn bộ Skin trong game")]
    // Tạo một danh sách (List) chứa hàng loạt các SkinItem ở trên
    public List<SkinItem> allSkins = new List<SkinItem>();
}
