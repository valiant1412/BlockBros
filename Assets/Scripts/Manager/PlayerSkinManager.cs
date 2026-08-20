using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    [Header("Cáº¥u hÃ¬nh")]
    [SerializeField] private int playerIndex;
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private SkinDatabaseSO skinDatabase;

    void Awake()
    {
        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>(true);
        }

        LoadAndApplyPlayerSkin();
    }
    void OnEnable()
    {
        SkinShopManager.OnSkinChanged += ApplyMaterial;
    }
    void OnDisable()
    {
        SkinShopManager.OnSkinChanged -= ApplyMaterial;
    }
    void LoadAndApplyPlayerSkin()
    {
        var currentSkin = SaveManager.Instance.gameData.currentSkin[playerIndex];
        foreach (var data in skinDatabase.allSkins)
        {
            if (data.skinID == currentSkin)
            {
                ApplyMaterialLocal(data.skinMaterial);
                return;
            }
        }
    }
    void ApplyMaterial(int targetPlayerIndex, Material mat)
    {
        if (targetPlayerIndex != playerIndex) return;
        ApplyMaterialLocal(mat);
    }
    void ApplyMaterialLocal(Material mat)
    {
        if (playerRenderer != null && mat != null)
        {
            playerRenderer.material = mat;
        }
    }
}
