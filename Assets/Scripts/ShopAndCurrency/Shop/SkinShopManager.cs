using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkinShopManager : MonoBehaviour
{
    public static SkinShopManager Instance;

    public static Action<int, Material> OnSkinChanged;

    [SerializeField] private GameObject skinInforPrefabs;

    [SerializeField] private Transform viewContent;

    [SerializeField] private SkinDatabaseSO skinDatabase;

    [Header("Trạng thái hiện tại")]
    // 0 = Đang xem Shop của Player 1 | 1 = Đang xem Shop của Player 2
    public int currentViewingPlayerIndex = 0;



    private SkinButtonUI[] spawnedButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

    }
    private void Start()
    {
        GenerateSkinShop();
    }
    private void GenerateSkinShop()
    {
        if (skinInforPrefabs == null || viewContent == null || skinDatabase == null) return;
        int totalSkin = skinDatabase.allSkins.Count;
        spawnedButton = new SkinButtonUI[totalSkin];
        Debug.Log(totalSkin);
        for (int i = 0; i < totalSkin; i++)
        {
            GameObject skinShop = Instantiate(skinInforPrefabs, viewContent, false);
            var skinButtonUIScript = skinShop.GetComponent<SkinButtonUI>();
            skinButtonUIScript.Setup(skinDatabase.allSkins[i]);
            spawnedButton[i] = skinButtonUIScript;
        }
    }
    public void SwitchPlayerTag(int playerIndex)
    {
        Debug.Log(playerIndex);
        currentViewingPlayerIndex = playerIndex;
        RefreshAllShop();
    }
    public void RefreshAllShop()
    {
        if (spawnedButton == null || spawnedButton.Count() == 0) return;
        foreach (var btn in spawnedButton)
        {
            if (btn != null) btn.ResetState();
        }
    }
}
