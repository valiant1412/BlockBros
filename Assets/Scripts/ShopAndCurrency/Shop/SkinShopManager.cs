using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopManager : MonoBehaviour
{
    public static SkinShopManager Instance;
    public static event Action<int, string> OnSkinChanged;

    [Header("Danh sách skin")]
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private Transform viewContent;
    [SerializeField] private ScrollRect skinScrollRect;
    [SerializeField] private SkinDatabaseSO skinDatabase;

    [Header("Thông tin skin đang chọn")]
    [Tooltip("Khung trắng phía trên Shop. Ảnh skin sẽ được tạo bên trong khung này.")]
    [SerializeField] private Image previewContent;

    [Header("Nút hành động cố định ở đáy")]
    [SerializeField] private Image bottomActionImage;
    [SerializeField] private TextMeshProUGUI bottomActionLabel;
    [SerializeField] private Sprite greenButton;
    [SerializeField] private Sprite yellowButton;

    [Header("Trạng thái hiện tại")]
    // 0 = Player 1 | 1 = Player 2
    public int currentViewingPlayerIndex;
    [Header("Số tiền")]
    [SerializeField] private TextMeshProUGUI money;

    [Header("Popup")]
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject unlockUI;
    [SerializeField] private GameObject canBuyUI;

    private readonly List<GameObject> spawnedBoards = new();
    private SkinButtonUI[] spawnedButtons;
    private Button bottomActionButton;
    private Image previewImage;
    private string selectedSkinId;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (viewContent == null && skinScrollRect != null) viewContent = skinScrollRect.content;

        SetupFixedActionButton();
        GenerateSkinShop();
    }

    private void OnEnable()
    {
        RefreshMoney();
        RefreshAllShop();
    }

    private void GenerateSkinShop()
    {
        if (boardPrefab == null || viewContent == null || skinDatabase == null)
        {
            Debug.LogWarning("Skin Shop cần Board Prefab, Content và Skin Database.", this);
            return;
        }

        ClearGeneratedBoards();
        spawnedButtons = new SkinButtonUI[skinDatabase.allSkins.Count];

        for (int i = 0; i < skinDatabase.allSkins.Count; i++)
        {
            GameObject board = Instantiate(boardPrefab, viewContent, false);
            SkinButtonUI boardUI = board.GetComponent<SkinButtonUI>();
            if (boardUI == null)
            {
                Debug.LogError("Board Prefab phải có SkinButtonUI.", board);
                Destroy(board);
                continue;
            }

            boardUI.Setup(skinDatabase.allSkins[i]);
            spawnedButtons[i] = boardUI;
            spawnedBoards.Add(board);
        }

        EnsureSelectedSkin();
        RefreshMoney();
        RefreshAllShop();
    }

    private void ClearGeneratedBoards()
    {
        foreach (GameObject board in spawnedBoards)
        {
            if (board != null) Destroy(board);
        }

        spawnedBoards.Clear();
    }

    public void SelectSkin(string skinId)
    {
        if (skinDatabase == null || !skinDatabase.allSkins.Any(skin => skin.skinID == skinId)) return;

        selectedSkinId = skinId;
        RefreshMoney();
        RefreshAllShop();
    }

    public void RefreshAllShop()
    {
        if (skinDatabase == null || SaveManager.Instance == null || SaveManager.Instance.gameData == null) return;

        EnsureSelectedSkin();
        SkinItem selectedSkin = GetSelectedSkin();
        if (selectedSkin == null) return;

        string equippedSkinId = GetEquippedSkinId();
        bool selectedIsOwned = IsOwned(selectedSkin.skinID);
        bool selectedIsEquipped = selectedSkin.skinID == equippedSkinId;

        if (spawnedButtons != null)
        {
            foreach (SkinButtonUI board in spawnedButtons)
            {
                if (board == null) continue;
                board.RefreshState(selectedSkinId, IsOwned(board.SkinId), board.SkinId == equippedSkinId);
            }
        }

        RefreshPreview(selectedSkin);
        RefreshBottomAction(selectedSkin, selectedIsOwned, selectedIsEquipped);
    }
    public void RefreshMoney()
    {
        int currentGold = SaveManager.Instance.gameData.totalGold;

        // 2. Ném tiền thô vào máy Format, sau đó in lên màn hình
        money.text = GoldFormatter.FormatGold(currentGold);
    }

    private void SetupFixedActionButton()
    {
        if (bottomActionImage == null) return;

        bottomActionButton = bottomActionImage.GetComponent<Button>();
        if (bottomActionButton == null) bottomActionButton = bottomActionImage.gameObject.AddComponent<Button>();

        bottomActionButton.targetGraphic = bottomActionImage;
        bottomActionButton.onClick.RemoveListener(ExecuteSelectedSkinAction);
        bottomActionButton.onClick.AddListener(ExecuteSelectedSkinAction);

        if (bottomActionLabel == null)
        {
        }//bottomActionLabel = CreateActionLabel(bottomActionImage.transform);
    }



    private void ExecuteSelectedSkinAction()
    {
        SkinItem selectedSkin = GetSelectedSkin();
        if (selectedSkin == null || SaveManager.Instance == null || SaveManager.Instance.gameData == null) return;

        GameData gameData = SaveManager.Instance.gameData;
        if (IsOwned(selectedSkin.skinID))
        {
            if (GetEquippedSkinId() == selectedSkin.skinID) return;

            gameData.currentSkin[currentViewingPlayerIndex] = selectedSkin.skinID;
            SaveManager.Instance.SaveGame();
            NotifySkinChanged(currentViewingPlayerIndex, selectedSkin.skinID);
        }
        else
        {

            //open pop up

            var popupInstance = popup.GetComponent<PopUp>();
            popupInstance.title.text = "Do you want to buy " + selectedSkin.skinName;
            popupInstance.previewContent.sprite = selectedSkin.skinIcon;
            popupInstance.price.text = "-" + selectedSkin.price.ToString();
            popup.SetActive(true);

            // if (EconomyManager.Instance == null) return;

            // EconomyManager.Instance.BuySkin(selectedSkin.price, out bool canBuy);
            // if (!canBuy) return;

            // gameData.Inventories[currentViewingPlayerIndex].OwnedSkins.Add(selectedSkin.skinID);
            // SaveManager.Instance.SaveGame();
        }
        RefreshMoney();
        RefreshAllShop();
    }

    private void RefreshBottomAction(SkinItem selectedSkin, bool isOwned, bool isEquipped)
    {
        if (bottomActionImage != null)
        {
            Sprite targetSprite = isOwned ? yellowButton : greenButton;
            if (targetSprite != null) bottomActionImage.sprite = targetSprite;
        }

        if (bottomActionButton != null) bottomActionButton.interactable = !isEquipped;
        if (bottomActionLabel == null) return;

        if (isEquipped) bottomActionLabel.text = "ĐANG TRANG BỊ";
        else if (isOwned) bottomActionLabel.text = "TRANG BỊ";
        else bottomActionLabel.text = $"MUA  ★ {GoldFormatter.FormatGold(selectedSkin.price)}";
    }

    private void RefreshPreview(SkinItem selectedSkin)
    {
        if (previewContent == null) return;




        previewContent.preserveAspect = true;
        previewContent.raycastTarget = false;


        previewContent.sprite = selectedSkin.skinIcon;
    }
    private void EnsureSelectedSkin()
    {
        if (GetSelectedSkin() != null) return;
        selectedSkinId = GetEquippedSkinId();

        if (GetSelectedSkin() == null && skinDatabase != null && skinDatabase.allSkins.Count > 0)
        {
            selectedSkinId = skinDatabase.allSkins[0].skinID;
        }
    }

    public SkinItem GetSelectedSkin()
    {
        return skinDatabase == null ? null : skinDatabase.allSkins.FirstOrDefault(skin => skin.skinID == selectedSkinId);
    }

    private string GetEquippedSkinId()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.gameData == null ||
            SaveManager.Instance.gameData.currentSkin == null ||
            currentViewingPlayerIndex >= SaveManager.Instance.gameData.currentSkin.Length)
        {
            return string.Empty;
        }

        return SaveManager.Instance.gameData.currentSkin[currentViewingPlayerIndex];
    }

    private bool IsOwned(string skinId)
    {
        GameData gameData = SaveManager.Instance.gameData;
        return gameData.Inventories != null && currentViewingPlayerIndex < gameData.Inventories.Length &&
               gameData.Inventories[currentViewingPlayerIndex] != null &&
               gameData.Inventories[currentViewingPlayerIndex].OwnedSkins.Contains(skinId);
    }

    private void ScrollToTop()
    {
        if (skinScrollRect == null) return;
        Canvas.ForceUpdateCanvases();
        skinScrollRect.verticalNormalizedPosition = 1f;
    }

    public void NotifySkinChanged(int playerIndex, string skinId)
    {
        OnSkinChanged?.Invoke(playerIndex, skinId);
    }

    public void YesToBuySkin()
    {

        SkinItem selectedSkin = GetSelectedSkin();
        if (selectedSkin == null || SaveManager.Instance == null ||
            SaveManager.Instance.gameData == null ||
            EconomyManager.Instance == null)
        {
            return;
        }

        GameData gameData = SaveManager.Instance.gameData;

        EconomyManager.Instance.BuySkin(selectedSkin.price, out bool canBuy);

        // Không đủ tiền
        if (!canBuy)
        {
            popup.SetActive(false);
            canBuyUI.SetActive(true); // UI báo không đủ tiền
            return;
        }

        // Mua thành công
        if (!gameData.Inventories[currentViewingPlayerIndex]
                .OwnedSkins.Contains(selectedSkin.skinID))
        {
            gameData.Inventories[currentViewingPlayerIndex]
                .OwnedSkins.Add(selectedSkin.skinID);
        }

        SaveManager.Instance.SaveGame();

        popup.SetActive(false);
        var unlock = unlockUI.GetComponent<Unlock>();
        unlock.title.text = "Congratulation";
        unlock.previewContent.sprite = selectedSkin.skinIcon;
        unlockUI.SetActive(true); // UI báo mở khóa thành công
        // SFX thắng chỉ phát khi AudioManager đã sẵn sàng và vẫn tôn trọng cài đặt tắt SFX.
        if (AudioManager.instance != null) AudioManager.instance.PlayWin();

        RefreshMoney();
        RefreshAllShop();
    }

}
