using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Score : MonoBehaviour
{
    public float score =0;

    [SerializeField]  private float maxScore;
    void Update()
    {
        if(score == maxScore)
        {
            gameObject.SetActive(false);
        }
    }
    public void AddScore()
    {
        score +=1;
    }
}
