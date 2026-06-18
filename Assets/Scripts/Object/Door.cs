using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpened = false;

    void Update()
    {
        if (isOpened
        ) return;
        if (ScoreManager.Instance != null && ScoreManager.Instance.IsReachTheMaxPoint())
        {
            OpenDoor();
        }
    }
    void OpenDoor()
    {
        isOpened = true;
        gameObject.SetActive(false);

        Debug.Log("Cửa đã mở");
    }
}
