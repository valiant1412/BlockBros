using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{   
    [Header("Nhân vật game")]
    [SerializeField] private Player player;
    
    [SerializeField] private Transform otherPlayer;

    [Header("Layer game")]
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private LayerMask playerLayer;

    // Đã đổi tên để nhắc nhở: Tích chọn CẢ Ground và Stair ngoài Inspector vào ô này!
    [SerializeField] private LayerMask walkableLayer;

    [Header("Class cần thiết")]
    [SerializeField] private GameOver gameOver;

    [SerializeField] private Winzone Winzone;

     [SerializeField] private ToggleBanner toggleBanner;

    // CHỐT KHÓA: Đừng quên cái này để tránh lỗi spam phím bay lên trời
    public float moveDuration = 0.2f;

    private bool isMoving = false;

    private bool isGameOver = false;

    [SerializeField] private int maxDistance;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trap"))
        {
            gameOver.Lose();
        }
    }
    void Awake()
    {
        player = GetComponent<Player>();
        gameOver = FindAnyObjectByType<GameOver>(FindObjectsInactive.Include);
    }

    void Update()
    {
        if (!isMoving)
        {
            // Gác cổng: Đang di chuyển thì cấm nhận phím
            var currentPosition = player.transform.position;
            if (Input.GetKeyDown(KeyCode.A)) CheckIsAbleToMove(currentPosition, Vector3.left);
            else if (Input.GetKeyDown(KeyCode.D)) CheckIsAbleToMove(currentPosition, Vector3.right);
            else if (Input.GetKeyDown(KeyCode.W)) CheckIsAbleToMove(currentPosition, Vector3.forward);
            else if (Input.GetKeyDown(KeyCode.S)) CheckIsAbleToMove(currentPosition, Vector3.back);
        }

    }

    void CheckIsAbleToMove(Vector3 currentPosition, Vector3 direction)
    {
        // 1. TIA NGANG: Kiểm tra vật cản thẳng đứng (Tường, Khối, Người chơi khác)
        var isBlock = player.CheckBlockedByObject(currentPosition, direction, blockLayer);
        var isBlockByPlayer = player.CheckBlockedByPlayer(currentPosition, direction, playerLayer);

        // Nếu bị chặn ngang -> Kết thúc hàm, không cho đi, không bắn tia dọc nữa!
        if (isBlock || isBlockByPlayer) return;
        // kiểm tra tọa độ hiện tại của hai nhân vật đã cách xa nhau chưa
        // 2. CHUẨN BỊ TỌA ĐỘ
        var target = currentPosition + direction * 1f; // Tọa độ Pivot của ô tiếp theo
        // if (!IsDistanceAllowed(target))
        // {   
        //     return;
        // }
        Vector3 finalTarget = target;

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
                    return;
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
        isMoving = true;
        player.targetPosition = finalTarget;
        StartCoroutine(Move(currentPosition, direction, finalTarget));
    }
    // win
    // hai nhan vat di vao trong hop thi moi coi la win.
    IEnumerator Move(Vector3 currentPosition, Vector3 direction, Vector3 finalTarget)
    {
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            float percent = elapsedTime / moveDuration;
            var jumpHeight = 0.5f;
            float heightOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;

            var currentPos = Vector3.Lerp(currentPosition, finalTarget, percent);
            currentPos.y += heightOffset * 0.5f;

            elapsedTime += Time.deltaTime;
            player.transform.position = currentPos;
            yield return null;
        }
        player.transform.position = finalTarget;
        isMoving = false;
        Winzone.CheckWinCondition();
        if (isGameOver)
        {
            gameOver.Lose();
        }
    }
    bool IsDistanceAllowed(Vector3 futurePosition)
    {
        if(otherPlayer ==null) return false;

        float distanceX = Mathf.Abs(futurePosition.x - otherPlayer.position.x);
        float distanceZ = Mathf.Abs(futurePosition.z - otherPlayer.position.z);

        if (distanceX > maxDistance + 0.1f || distanceZ > maxDistance + 0.1f)
        {   
            toggleBanner.ShowWarning();
            return false; // Cho phép đi
        }
        
        return true;
    }
}