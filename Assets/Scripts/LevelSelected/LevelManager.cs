using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    //Description: : Lắng nghe khi nút được bấm -> Tắt bảng UI -> Mở Map ra cho khủng long chạy.
    public static LevelManager Instance;
    // Start is called before the first frame update


    [Header("Quản lí level")]
    public GameObject currentMap;

    [SerializeField] private GameObject levelSelectedUI;
    void Awake()
    {
        Instance = this;
    }


    // Update is called once per frame.
    public void StartLevel(string name)
    {
        //tắt UI
        levelSelectedUI.SetActive(false);

        // load map
        string mapName = "Maps/Level" + name;
        GameObject map = Resources.Load<GameObject>(mapName);
        if (currentMap != null) Destroy(currentMap);

        currentMap = Instantiate(map, Vector3.zero, Quaternion.identity);
        WinLoseManager.Instance.isGameEnded = false;
        PlayerPrefs.SetInt("LevelInPlay", int.Parse(name));
    }
    public void ReturnToMenu()
    {
        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.ResetMovement();
        }
        if (currentMap != null)
        {
            Destroy(currentMap);
            currentMap = null;
        }
        levelSelectedUI.SetActive(true);
        Time.timeScale = 1f;
    }

}
