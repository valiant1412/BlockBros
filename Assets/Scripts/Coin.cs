using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // Mặc định xoay quanh trục Y với tốc độ 100

    private Score score;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            AudioManager.instance.PlayCoin();
            score.AddScore();
        }
    }
    void Start()
    {
        score = FindObjectOfType<Score>();
    }
    void Update()
    {
        // Thực hiện xoay vật thể mỗi khung hình (frame)
        // Dùng Space.World để xoay theo trục của thế giới, hoặc Space.Self để xoay theo trục của chính vật thể
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
    }
}
