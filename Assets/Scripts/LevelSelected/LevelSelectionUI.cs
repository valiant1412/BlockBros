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
        int levelInPlay = SaveManager.Instance.gameData.CurrentLevel;
        int highestLevel = SaveManager.Instance.gameData.HighestLevel;

        if (levelInPlay == 0) levelInPlay = 1;
        if (highestLevel == 0) highestLevel = 1;

        // 2. Vòng lặp thần thánh: Vừa đẻ nút, vừa xài lại nút cũ
        for (int i = 1; i <= totalLevel; i++)
        {
            LevelSeleceted levelSelected;

            // KIỂM TRA TRÍ NHỚ: Trong bảng đã có sẵn cái nút thứ i chưa?
            if (i - 1 < viewContent.childCount)
            {
                // CÓ SẴN RỒI: Tái sử dụng luôn, tuyệt đối không Instantiate
                levelSelected = viewContent.GetChild(i - 1).GetComponent<LevelSeleceted>();
            }
            else
            {
                // CHƯA CÓ: Mới Instantiate để bù vào
                GameObject newButton = Instantiate(levelButonPrefab, viewContent);
                newButton.transform.localScale = Vector3.one;
                levelSelected = newButton.GetComponent<LevelSeleceted>();
            }

            // 3. Logic phân màu và trạng thái của bạn (đã được tinh gọn)
            string hexColor = ""; // Mặc định là chuỗi rỗng (dành cho nút bị Khóa)
            bool isPlayed = (i <= highestLevel); // Cứ nhỏ hơn hoặc bằng HighestLevel là được chơi

            if (i == levelInPlay)
            {
                hexColor = "#4E8C61"; // Màu xanh - Đang chơi
            }
            else if (isPlayed)
            {
                hexColor = "#FF842D"; // Màu cam - Đã chơi qua
            }

            // 4. Bơm dữ liệu mới vào cái nút
            levelSelected.SetupButton(i, isPlayed, hexColor);
        }
    }


}
