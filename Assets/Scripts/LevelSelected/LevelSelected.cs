using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSeleceted : MonoBehaviour
{
    [SerializeField] private Button myButton;
    [SerializeField] private TextMeshProUGUI levelText;

    private int myLevelIndex;

    void Start()
    {

    }
    public void SetupButton(int levelIndex, bool isPlayed, string hex)
    {
        myLevelIndex = levelIndex;
        levelText.text = levelIndex.ToString();


        myButton.interactable = isPlayed;
        if (isPlayed)
        {
            ChangeColor(hex);
        }
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnLevelButtonClicked);
    }

    private void OnLevelButtonClicked()
    {
        // chuyển màn
        LevelManagement.Instance.StartLevel(myLevelIndex.ToString());
    }

    private void ChangeColor(string hex)
    {
        Color convertedColor;
        if (ColorUtility.TryParseHtmlString(hex, out convertedColor))
        {
            // 3. Tiến hành đổi màu nút (Giống hệt cách 1 bài trước)
            ColorBlock cb = myButton.colors;

            cb.normalColor = convertedColor;      // Đổi màu hiển thị mặc định
                                                  // cb.highlightedColor = convertedColor; // Đổi cả màu khi hover nếu muốn
            myButton.colors = cb;
        }
    }
}
