using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen2 : MonoBehaviour
{

    private AsyncOperation asyncLoad;

    void Awake()
    {

    }
    void Start()
    {
        StartCoroutine(LoadGameInBackGround());
    }
    IEnumerator LoadGameInBackGround()
    {
        // 1. Lấy thông tin màn chơi cần load
        int currentLevel = PlayerPrefs.GetInt("LevelInPlay");

        // 2. Bắt đầu load ngầm
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentLevel.ToString());

        // Khóa cửa, không cho tự động chuyển màn khi load xong
        asyncLoad.allowSceneActivation = false;

        // 3. BÍ QUYẾT: Check tiến độ < 0.9 thay vì !isDone
        while (asyncLoad.progress < 0.9f)
        {
            // Trong lúc này bạn có thể cập nhật thanh trượt Slider nếu có
            yield return null;
        }

        // --- LÚC NÀY GAME ĐÃ NẠP XONG 100% VÀO RAM ---

        // (Tùy chọn) Game của bạn rất nhẹ, nó sẽ load xong trong 0.01 giây. 
        // Hãy bắt nó đợi thêm 1 giây để người chơi kịp nhìn thấy chữ "Loading..."
        yield return new WaitForSeconds(1f);

        // 4. Mở khóa! Unity sẽ tự động chuyển cảnh mượt mà. 
        // KHÔNG CẦN gọi thêm lệnh SceneManager.LoadScene nào nữa!
        asyncLoad.allowSceneActivation = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
