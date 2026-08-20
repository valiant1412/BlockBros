using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null || WinLoseManager.Instance == null) return;

        WinLoseManager.Instance.NotifyPlayerExited(player);
    }
    // void OnTriggerEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         var player = collision.gameObject.GetComponent<Player>();
    //         player.SetState(PlayerState.Exit);
    //         player.gameObject.SetActive(false);
    //     }
    // }
}
