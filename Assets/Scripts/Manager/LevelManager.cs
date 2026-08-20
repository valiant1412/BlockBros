using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    //Description: : Láº¯ng nghe khi nÃºt Ä‘Æ°á»£c báº¥m -> Táº¯t báº£ng UI -> Má»Ÿ Map ra cho khá»§ng long cháº¡y.
    public static LevelManager Instance;
    // Start is called before the first frame update


    [Header("Quáº£n lÃ­ level")]
    public GameObject currentMap;

    [SerializeField] private GameObject levelSelectedUI;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        var currentLevel = SaveManager.Instance.gameData.CurrentLevel;
        LoadLevel(currentLevel, false);
    }


    // Update is called once per frame.
    public void StartLevel(string name)
    {
        TryStartLevel(name);
    }

    public bool TryStartLevel(string name)
    {
        if (!int.TryParse(name, out int levelIndex) || levelIndex < 1)
        {
            Debug.LogError("Invalid level name: " + name);
            return false;
        }

        return LoadLevel(levelIndex, true);
    }

    private bool LoadLevel(int levelIndex, bool startGameplay)
    {
        string mapName = "Maps/Level" + levelIndex;
        GameObject map = Resources.Load<GameObject>(mapName);
        if (map == null)
        {
            Debug.LogError("Level prefab was not found at Resources/" + mapName);
            return false;
        }

        // Only change state after the requested map is known to exist.
        if (levelSelectedUI != null) levelSelectedUI.SetActive(false);
        if (currentMap != null) Destroy(currentMap);


        currentMap = Instantiate(map, Vector3.zero, Quaternion.identity);
        LevelContext levelContext = currentMap.GetComponent<LevelContext>();
        if (levelContext != null) levelContext.PrepareRuntimeLevel();
        if (levelContext == null || !levelContext.HasValidSpawnPoints)
        {
            Debug.LogError("Level " + levelIndex + " needs a LevelContext with two spawn points.");
            Destroy(currentMap);
            currentMap = null;
            return false;
        }

        GameManager.Instance.SetupPlayer(levelContext);
        if (WinLoseManager.Instance != null)
        {
            WinLoseManager.Instance.ResetResult();
        }

        GameManager.Instance.SetGameplayActive(startGameplay);

        SaveManager.Instance.gameData.CurrentLevel = levelIndex;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool HasLevel(int levelIndex)
    {
        return levelIndex > 0 && Resources.Load<GameObject>("Maps/Level" + levelIndex) != null;
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
        if (levelSelectedUI != null) levelSelectedUI.SetActive(true);
        Time.timeScale = 1f;
    }

}
