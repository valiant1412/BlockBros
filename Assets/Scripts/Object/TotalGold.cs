using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TotalGold : MonoBehaviour
{
    public TextMeshProUGUI banner;

    private void OnEnable()
    {
        EconomyManager.OnGoldChanged += UpdateGold;
        UpdateGold();
    }
    private void OnDisable()
    {
        EconomyManager.OnGoldChanged -= UpdateGold;
    }
    public void UpdateGold()
    {
        int currentGold = SaveManager.Instance.gameData.totalGold;

        // 2. Ném tiền thô vào máy Format, sau đó in lên màn hình
        banner.text = GoldFormatter.FormatGold(currentGold);
    }

}
