using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayRefers : MonoBehaviour
{
    public int highestLevel;
    public int currentLevel;

    void Start()
    {
        PlayerPrefs.SetInt("HighestLevel", highestLevel);
        PlayerPrefs.SetInt("LevelInPlay", currentLevel);
    }
}
