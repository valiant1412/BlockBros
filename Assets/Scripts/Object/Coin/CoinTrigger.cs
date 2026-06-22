using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            AudioManager.instance.PlayCoin();
            ScoreManager.Instance.AddScore();
            HapticManager.LightTaptic();
        }
    }
}
