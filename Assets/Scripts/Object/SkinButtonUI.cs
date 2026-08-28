using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Một Board trong danh sách skin. Board chỉ dùng để chọn skin;
/// việc mua/trang bị luôn thực hiện bằng nút cố định ở cuối Shop.
/// </summary>
public class SkinButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skinName;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI price;

    private SkinItem skinItem;
    private Button selectButton;
    private Outline selectedOutline;

    public string SkinId => skinItem != null ? skinItem.skinID : string.Empty;

    public void Setup(SkinItem item)
    {
        skinItem = item;

        if (skinName != null) skinName.text = skinItem.skinName;
        if (icon != null) icon.sprite = skinItem.skinIcon;

        ConfigureAsSelectableBoard();
    }

    private void ConfigureAsSelectableBoard()
    {
        // Board chỉ có Image nền. Tạo Button lúc chạy giúp prefab gọn và không cần
        // đặt nút mua nhỏ bên trong từng Board.
        selectButton = GetComponent<Button>();
        if (selectButton == null) selectButton = gameObject.AddComponent<Button>();

        selectButton.targetGraphic = GetComponent<Image>();
        selectButton.onClick.RemoveListener(SelectThisSkin);
        selectButton.onClick.AddListener(SelectThisSkin);

        // Các Graphic con không chặn raycast của Button ở Board nền.
        foreach (Graphic graphic in GetComponentsInChildren<Graphic>(true))
        {
            if (graphic.gameObject != gameObject) graphic.raycastTarget = false;
        }

        selectedOutline = GetComponent<Outline>();
        if (selectedOutline == null) selectedOutline = gameObject.AddComponent<Outline>();
        selectedOutline.effectColor = new Color(1f, 0.66f, 0.08f, 1f);
        selectedOutline.effectDistance = new Vector2(5f, -5f);
        selectedOutline.useGraphicAlpha = false;
    }

    private void SelectThisSkin()
    {
        if (SkinShopManager.Instance != null) SkinShopManager.Instance.SelectSkin(SkinId);
    }

    public void RefreshState(string selectedSkinId, bool isOwned, bool isEquipped)
    {
        if (selectedOutline != null) selectedOutline.enabled = SkinId == selectedSkinId;

        if (price == null || skinItem == null) return;

        if (isEquipped) price.text = "ĐANG TRANG BỊ";
        else if (isOwned) price.text = "✓ ĐÃ CÓ";
        else price.text = $"★ {GoldFormatter.FormatGold(skinItem.price)}";
    }
}
