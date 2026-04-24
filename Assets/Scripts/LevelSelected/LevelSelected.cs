using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSeleceted : MonoBehaviour
{
    [SerializeField] private GameObject levelButonPrefab;

    [SerializeField] private Transform viewContent;

    public int totalLevel;
    // Start is called before the first frame update
    void Start()
    {
        GenerateLevels();
    }
    void GenerateLevels()
    {
        int levelInPlay = PlayerPrefs.GetInt("LevelInPlay");
        if (levelInPlay == 0)
        {
            levelInPlay = 1;
        }
        for (int i = 1; i <= totalLevel; i++)
        {
            GameObject newButton = Instantiate(levelButonPrefab, viewContent);
            Outline buttonOutline = newButton.GetComponent<Outline>();
            TextMeshProUGUI btnText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = i.ToString();
                string btnContent = btnText.text;
                Button btnComponent = newButton.GetComponent<Button>();

                btnComponent.onClick.AddListener(() =>
                {
                    PlayerPrefs.SetInt("LevelInPlay", Convert.ToInt32(btnContent));
                    SceneManager.LoadScene("Loading2");
                    //SceneManager.LoadScene(btnContent);
                    //set level in play

                });
                buttonOutline.enabled = false;
                var highestLevel = PlayerPrefs.GetInt("HighestLevel");
                if (highestLevel == 0)
                {
                    highestLevel = 1;
                }
                // kiểm tra màn đã chơi chưa
                if (i <= highestLevel)
                {
                    Button btn = newButton.GetComponent<Button>();
                    btnComponent.interactable = true;
                    if (i == highestLevel)
                    {
                        ChangeColor(btn, "#FF842D");
                    }
                    else
                    {
                        ChangeColor(btn, "#4E8C61");
                    }

                    if (i == levelInPlay)
                    {
                        buttonOutline.enabled = true;
                    }
                }
                else
                {
                    btnComponent.interactable = false;
                }
            }
        }
    }

    void ChangeColor(Button btn, string hex)
    {
        Color convertedColor;
        if (ColorUtility.TryParseHtmlString(hex, out convertedColor))
        {
            // 3. Tiến hành đổi màu nút (Giống hệt cách 1 bài trước)
            ColorBlock cb = btn.colors;

            cb.normalColor = convertedColor;      // Đổi màu hiển thị mặc định
                                                  // cb.highlightedColor = convertedColor; // Đổi cả màu khi hover nếu muốn
            btn.colors = cb;
        }
    }
}
