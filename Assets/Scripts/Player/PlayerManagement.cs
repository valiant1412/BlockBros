using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{   
    [Header("Nhân vật game")]
    
    [Header("Layer game")]
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private LayerMask playerLayer;

    // Đã đổi tên để nhắc nhở: Tích chọn CẢ Ground và Stair ngoài Inspector vào ô này!
    [SerializeField] private LayerMask walkableLayer;

    [Header("Class cần thiết")]

    public float moveDuration = 0.2f;

    private bool isMoving = false;

    private bool isGameOver = false;

    [SerializeField] private int maxDistance;

    void Awake()
    {
    }

    public bool IsBlocked(Player player, Vector3 currentPosition, Vector3 direction, out Vector3 finalTarget)
    {
        // 1. TIA NGANG: Kiểm tra vật cản thẳng đứng (Tường, Khối, Người chơi khác)
        var isBlock = player.CheckBlockedByObject(currentPosition, direction, blockLayer);
        var isBlockByPlayer = player.CheckBlockedByPlayer(currentPosition, direction, playerLayer);

        // Nếu bị chặn ngang -> Kết thúc hàm, không cho đi, không bắn tia dọc nữa!

        if (isBlock || isBlockByPlayer)
        {
            finalTarget = currentPosition;
            return true;
        }

        // 2. CHUẨN BỊ TỌA ĐỘ
        var target = currentPosition + direction * 1f; // Tọa độ Pivot của ô tiếp theo

        finalTarget = target;

        Collider playerCollider = player.GetComponentInChildren<Collider>();
        float distanceToBottom = 0f;
        Vector3 centerPos = currentPosition;

        if (playerCollider != null)
        {
            distanceToBottom = currentPosition.y - playerCollider.bounds.min.y;
            centerPos = playerCollider.bounds.center; // Lấy Tâm THỰC SỰ của khối
        }

        // 3. TIA DỌC: Dời súng vào CHÍNH GIỮA ô tiếp theo (Tránh bắn vào khe nứt)
        Vector3 targetCenter = centerPos + direction * 1f;
        Vector3 rayCastingPoint = new Vector3(targetCenter.x, target.y + 10f, targetCenter.z);


        // BẮN TIA KIỂM TRA ĐỊA HÌNH
        if (Physics.Raycast(rayCastingPoint, Vector3.down, out RaycastHit hit, 20f, walkableLayer))
        {
            // Tính độ sâu của cú nhảy (Đất hiện tại - Đất phía trước)
            float currentGroundY = currentPosition.y - distanceToBottom;
            float dropDepth = currentGroundY - hit.point.y;

            // Nhảy an toàn (Bằng phẳng hoặc dốc)
            finalTarget = hit.point;
            finalTarget.y += distanceToBottom;
            if (Physics.Raycast(rayCastingPoint, Vector3.down, out RaycastHit newHit, 20f, playerLayer))
            {
                var besidePlayer = newHit.collider.gameObject.GetComponent<Player>();
                if (besidePlayer.isBlockByObject)
                {   
                    return true;
                }
            }
        }
        else
        {
            Vector3 abyssTarget = target;
            abyssTarget.y = -20f;

            finalTarget = abyssTarget;
            isGameOver = true;
        }

        // Chỉ khi mọi thứ ok mới gọi Coroutine di chuyển
        return false;
    }

}