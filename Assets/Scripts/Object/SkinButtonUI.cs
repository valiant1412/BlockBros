
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
public class SkinButtonUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI skinName;


    [SerializeField] private Image outlineImage;
    [SerializeField] private Image icon;

    [SerializeField] private Button actionBtn;

    [SerializeField] private TextMeshProUGUI price;

    private SkinItem mySkinItem;
    private string skin_ID;
    public void Setup(SkinItem skinItem)
    {
        mySkinItem = skinItem;
        skinName.text = mySkinItem.skinName;
        icon.sprite = mySkinItem.skinIcon;
        price.text = mySkinItem.price.ToString();
        skin_ID = mySkinItem.skinID;

        actionBtn.onClick.RemoveAllListeners();
        actionBtn.onClick.AddListener(OnClickButton);

        ResetState();
    }

    public void OnClickButton()
    {
        GameData gameData = SaveManager.Instance.gameData;
        int currentViewIndex = SkinShopManager.Instance.currentViewingPlayerIndex;
        bool isOwned = gameData.Inventories[currentViewIndex].OwnedSkins.Contains(skin_ID);
        bool isEquipped = gameData.currentSkin[currentViewIndex] == skin_ID;
        if (isOwned)
        {
            if (!isEquipped)
            {
                // doi sang current.
                gameData.currentSkin[currentViewIndex] = mySkinItem.skinID;
            }
        }
        else
        {
            // trong truong hop mua.
            int price = mySkinItem.price;
            EconomyManager.Instance.BuySkin(price, out bool isAbleToBuy);
            if (!isAbleToBuy)
            {
                Debug.Log("Khong the mua");
            }
            else
            {
                gameData.Inventories[currentViewIndex].OwnedSkins.Add(mySkinItem.skinID);
            }
        }
        SaveManager.Instance.SaveGame();
        SkinShopManager.Instance.RefreshAllShop();
        SkinShopManager.OnSkinChanged?.Invoke(currentViewIndex, mySkinItem.skinMaterial);
    }
    public void ResetState()
    {
        GameData gameData = SaveManager.Instance.gameData;
        int currentViewIndex = SkinShopManager.Instance.currentViewingPlayerIndex;
        bool isOwned = gameData.Inventories[currentViewIndex].OwnedSkins.Contains(skin_ID);
        var currentSkin = gameData.currentSkin[currentViewIndex];

        bool isEquipped = currentSkin == skin_ID;

        if (isOwned)
        {
            if (!isEquipped)
            {
                price.text = "SELECT";
                if (outlineImage != null) outlineImage.color = Color.yellow;
                actionBtn.interactable = true;
            }
            else
            {
                price.text = "EQUIPPED";
                if (outlineImage != null) outlineImage.color = Color.green;
                actionBtn.interactable = false;
            }


        }
        else
        {
            price.text = GoldFormatter.FormatGold(mySkinItem.price);
            if (outlineImage != null) outlineImage.color = Color.gray;
            actionBtn.interactable = true;
        }
    }
}
