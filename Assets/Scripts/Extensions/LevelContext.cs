using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelContext : MonoBehaviour
{
    [SerializeField] public Transform player1Spawn;
    [SerializeField] public Transform player2Spawn;
    [SerializeField] public Transform exitZone;
    [SerializeField] private List<Transform> exitZones = new List<Transform>();

    public bool HasValidSpawnPoints => player1Spawn != null && player2Spawn != null;

    public void PrepareRuntimeLevel()
    {
        ResolveLegacySpawnPoints();
        SetupExitZones();
    }

    private void ResolveLegacySpawnPoints()
    {
        if (HasValidSpawnPoints) return;

        Player[] legacyPlayers = GetComponentsInChildren<Player>(true);
        foreach (Player legacyPlayer in legacyPlayers)
        {
            if (legacyPlayer.name == "Player1") player1Spawn = legacyPlayer.transform;
            if (legacyPlayer.name == "Player2") player2Spawn = legacyPlayer.transform;
        }

        // Old levels still contain player prefabs. Their transforms become temporary spawn points.
        foreach (Player legacyPlayer in legacyPlayers)
        {
            legacyPlayer.gameObject.SetActive(false);
        }
    }

    private void SetupExitZones()
    {
        if (exitZone != null && !exitZones.Contains(exitZone)) exitZones.Add(exitZone);

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (!child.name.StartsWith("Winzone")) continue;
            if (!exitZones.Contains(child)) exitZones.Add(child);
        }

        foreach (Transform zone in exitZones)
        {
            if (zone == null) continue;

            BoxCollider boxCollider = zone.GetComponent<BoxCollider>();
            if (boxCollider != null) boxCollider.isTrigger = true;
            if (zone.GetComponent<WinTrigger>() == null) zone.gameObject.AddComponent<WinTrigger>();
        }
    }
    void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetupForNewLevel(FindTotalCoin());
        }
    }
    int FindTotalCoin()
    {
        // QuÃ©t toÃ n bá»™ cÃ¡c váº­t thá»ƒ con bÃªn trong Map nÃ y
        Coin[] allCoinsInMap = GetComponentsInChildren<Coin>(true);

        // Sá»‘ lÆ°á»£ng xu chÃ­nh lÃ  Ä‘á»™ dÃ i cá»§a danh sÃ¡ch vá»«a tÃ¬m Ä‘Æ°á»£c! KhÃ´ng cáº§n vÃ²ng láº·p.
        int coinCount = allCoinsInMap.Length;
        return coinCount;
    }
}
