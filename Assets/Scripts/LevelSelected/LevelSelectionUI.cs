using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject levelButonPrefab;

    [SerializeField] private Transform viewContent;

    public int totalLevel;
    // Start is called before the first frame update
    void OnEnable()
    {
        GenerateLevels();
    }
    void GenerateLevels()
    {
        foreach (Transform child in viewContent)
        {
            Destroy(child.gameObject);
        }


        int levelInPlay = PlayerPrefs.GetInt("LevelInPlay");
        if (levelInPlay == 0)
        {
            levelInPlay = 1;
        }
        for (int i = 1; i <= totalLevel; i++)
        {
            string hexColor = "";
            bool isPlayed = true;
            GameObject newButton = Instantiate(levelButonPrefab, viewContent);
            // Ép Scale về 1 để tránh lỗi nút bị to ra/nhỏ đi bất thường của Unity UI
            newButton.transform.localScale = Vector3.one;

            LevelSeleceted levelSeleceted = newButton.GetComponent<LevelSeleceted>();

            // thêm số vào trong nút, nếu người chơi chưa đến màn đó, khóa nó lại.
            var highestLevel = PlayerPrefs.GetInt("HighestLevel");
            if (highestLevel == 0)
            {
                highestLevel = 1;
            }
            if (i > highestLevel)
            {
                isPlayed = false;
            }
            else
            {
                isPlayed = true;
                hexColor = "#FF842D";
            }

            // điều chỉnh màu của button, nếu như hiện tại thì là màu xanh, đã chơi là màu cam.
            if (i == levelInPlay)
            {
                isPlayed = true;
                hexColor = "#4E8C61";
            }
            levelSeleceted.SetupButton(i, isPlayed, hexColor);
        }
    }

}
