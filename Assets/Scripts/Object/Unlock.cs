using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Unlock : MonoBehaviour
{
    // Đặt confetti lên trước UI Canvas để hạt không bị các panel che khuất.
    private const int ConfettiSortingOrder = 100;

    [SerializeField] private GameObject winConfettiPrefab;
    [SerializeField, Min(0f)] private float confettiDuration = 5f;
    // Thu nhỏ root của prefab để tái sử dụng cùng một confetti cho UI Unlock.
    [SerializeField, Range(0.05f, 1f)] private float confettiScale = 0.35f;

    [SerializeField] public TextMeshProUGUI title;

    [SerializeField] public Image previewContent;

    void OnEnable()
    {
        PlayWinConfetti();
    }
    private void PlayWinConfetti()
    {
        if (winConfettiPrefab == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        Vector3 spawnPosition = mainCamera != null
            ? mainCamera.transform.position + mainCamera.transform.forward * 8f
            : Vector3.zero;
        Quaternion spawnRotation = mainCamera != null ? mainCamera.transform.rotation : Quaternion.identity;

        // Spawn trước camera để hiệu ứng luôn xuất hiện trên vùng nhìn thấy của người chơi.
        GameObject confetti = Instantiate(winConfettiPrefab, spawnPosition, spawnRotation);
        // Ghi đè scale của prefab mà không làm thay đổi asset gốc.
        confetti.transform.localScale = Vector3.one * confettiScale;

        foreach (ParticleSystem particleSystem in confetti.GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule main = particleSystem.main;
            // Confetti vẫn chạy khi popup làm Time.timeScale bằng 0.
            main.useUnscaledTime = true;

            ParticleSystemRenderer particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            if (particleRenderer != null)
            {
                // Đẩy sorting order để particle render bên trên UI thông thường.
                particleRenderer.sortingOrder = ConfettiSortingOrder;
            }

            particleSystem.Play(true);
        }

        // Tự hủy instance sau thời lượng thực để tránh tích lũy GameObject mỗi lần mở Unlock.
        StartCoroutine(DestroyAfterRealtime(confetti, confettiDuration));
    }

    private static IEnumerator DestroyAfterRealtime(GameObject effect, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        if (effect != null)
        {
            Destroy(effect);
        }
    }
}
