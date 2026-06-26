using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static EconomyManager Instance;

    public static Action OnGoldChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void AddMoney(int money)
    {
        int totalGold = SaveManager.Instance.gameData.totalGold;
        totalGold += money;
        SaveManager.Instance.gameData.totalGold = totalGold;
        SaveManager.Instance.SaveGame();

        OnGoldChanged?.Invoke();

    }
    public void BuySkin(int price, out bool isAbleToBuy)
    {
        int totalGold = SaveManager.Instance.gameData.totalGold;
        isAbleToBuy = ValidateMoney(price);
        if (!isAbleToBuy)
        {
            isAbleToBuy = ValidateMoney(price);
            return;
        }
        else
        {
            totalGold -= price;
            SaveManager.Instance.gameData.totalGold = totalGold;
            SaveManager.Instance.SaveGame();

            OnGoldChanged?.Invoke();
        }

    }
    public bool ValidateMoney(int price)
    {
        int totalGold = SaveManager.Instance.gameData.totalGold;
        if (totalGold < price) return false;
        return true;
    }

}
