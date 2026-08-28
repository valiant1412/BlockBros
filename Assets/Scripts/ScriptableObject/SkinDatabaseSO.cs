using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // BẮT BUỘC: Thần chú này giúp Unity vẽ class này ra ngoài Inspector
public class SkinItem
{
    public string skinID;
    public string skinName;
    public int price;
    public Sprite skinIcon;

    public Material knightBigMaterial; // Player 1
    public Material knightMaterial;    // Player 2

    public Material GetMaterialForPlayer(int playerIndex)
    {
        return playerIndex == 0 ? knightBigMaterial : knightMaterial;
    }
}

// 2. TẠO KHUÔN ĐÚC (Kế thừa ScriptableObject)
[CreateAssetMenu(fileName = "SkinDatabase", menuName = "Shop/Skin Database")]
public class SkinDatabaseSO : ScriptableObject
{
    [Header("Kho chứa toàn bộ Skin trong game")]
    // Tạo một danh sách (List) chứa hàng loạt các SkinItem ở trên
    public List<SkinItem> allSkins = new List<SkinItem>();

    public bool TryGetMaterial(string skinID, int playerIndex, out Material material)
    {
        material = null;

        if (string.IsNullOrWhiteSpace(skinID)) return false;

        foreach (SkinItem skin in allSkins)
        {
            if (skin == null || skin.skinID != skinID) continue;

            material = skin.GetMaterialForPlayer(playerIndex);
            return material != null;
        }

        return false;
    }
}
