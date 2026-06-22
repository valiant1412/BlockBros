using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFooter : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject coinVisual;
    [SerializeField] private GameObject coinFooter;
    [SerializeField] private LayerMask layers;

    [SerializeField] private float heightOffset = 0.01f;

    void OnEnable()
    {
        SnapFooterToGround();
    }

    void SnapFooterToGround()
    {
        if (coinVisual == null || coinFooter == null)
        {
            Debug.LogError("Coin Prefab đang bị thiếu kéo thả object vào Script ở Inspector!");
            return;
        }
        // Đoạn code trước có thể đang bắn từ trục gốc (this.transform.position) nên bị lệch.
        if (Physics.Raycast(coinVisual.transform.position, Vector3.down, out RaycastHit hit, 100f, layers))
        {
            // Bật cái bóng lên nếu nó đang tắt
            coinFooter.gameObject.SetActive(true);

            // THẦN CHÚ: Đặt cái bóng vào đúng điểm chạm (hit.point)
            // BUG 2 (Ẩn): heightOffset quá nhỏ (0.01) khiến nó bị trùng mặt với mặt cỏ. Hãy tăng nó lên.
            coinFooter.transform.position = hit.point + new Vector3(0, heightOffset, 0);

            // Ép cái bóng nằm phẳng theo bề mặt nó chạm vào

            coinFooter.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(90f, 0f, 0f);

        }
        else
        {
            // Nếu dưới chân đồng xu là vực thẳm -> Tắt bóng đi
            coinFooter.gameObject.SetActive(false);
        }
    }
    public void DisableCoin()
    {
        gameObject.SetActive(false);
    }
}
