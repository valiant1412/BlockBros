using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int currentPoint = 0;
    public int maxPoint = 0;

    public static ScoreManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetupForNewLevel(int totalCoinInMap)
    {
        currentPoint = 0;
        maxPoint = totalCoinInMap;

        Debug.Log("Map đã được set up lại max score mới");
    }
    public void AddScore()
    {
        currentPoint++;
    }
    public bool IsReachTheMaxPoint()
    {
        return (currentPoint >= maxPoint && maxPoint > 0);
    }
}
