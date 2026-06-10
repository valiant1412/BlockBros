using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    public static PlayerMoving Instance;
    [Header("Nhân vật game")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;
    [Header("Layer game")]

    // Đã đổi tên để nhắc nhở: Tích chọn CẢ Ground và Stair ngoài Inspector vào ô này!
    [SerializeField] private LayerMask walkableLayer;

    [Header("Class cần thiết")]

    [SerializeField] private ToggleBanner toggleBanner;

    [SerializeField] private PlayerManager playerManagement;

    [Header("Âm thanh")]
    [SerializeField] private AudioSource audioSource;

    // CHỐT KHÓA: Đừng quên cái này để tránh lỗi spam phím bay lên trời
    public float moveDuration = 0.2f;

    private bool isMoving = false;

    private bool isGameOver = false;

    [SerializeField] private int maxDistance;

    public float minSwipeDistance = 50f;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (player1 == null || player2 == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        // Khi nhấc tay lên (hoặc nhả chuột trái)
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }

        if (!player1.isMoved && !player2.isMoved)
        {
            // Gác cổng: Đang di chuyển thì cấm nhận phím
            if (Input.GetKeyDown(KeyCode.A))
            {
                ProcessMoving(player1, player2, Vector3.left);

            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ProcessMoving(player1, player2, Vector3.right);
            }

            else if (Input.GetKeyDown(KeyCode.W))
            {
                ProcessMoving(player1, player2, Vector3.forward);

            }

            else if (Input.GetKeyDown(KeyCode.S))
            {
                ProcessMoving(player1, player2, Vector3.back);

            }

        }

    }

    private void DetectSwipe()
    {
        // Tính khoảng cách từ điểm Bắt đầu đến điểm Kết thúc
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        // Kiểm tra xem quãng đường vuốt có đủ dài không (Lớn hơn ngưỡng minSwipeDistance)
        if (swipeDelta.magnitude >= minSwipeDistance)
        {
            // Trị tuyệt đối để xem vuốt theo chiều Ngang hay chiều Dọc dài hơn
            float xDistance = Mathf.Abs(swipeDelta.x);
            float yDistance = Mathf.Abs(swipeDelta.y);

            if (xDistance > yDistance)
            {
                // VUỐT CHIỀU NGANG
                if (swipeDelta.x > 0)
                    ProcessMoving(player1, player2, Vector3.right); // Vuốt sang phải
                else
                    ProcessMoving(player1, player2, Vector3.left);  // Vuốt sang trái
            }
            else
            {
                // VUỐT CHIỀU DỌC
                if (swipeDelta.y > 0)
                    ProcessMoving(player1, player2, Vector3.forward); // Vuốt lên trên
                else
                    ProcessMoving(player1, player2, Vector3.back);    // Vuốt xuống dưới
            }
        }
    }
    void ProcessMoving(Player player1, Player player2, Vector3 direction)
    {
        var currentPosition1 = player1.transform.position;
        var currentPosition2 = player2.transform.position;

        bool isPlayer1Blocked = playerManagement.IsBlocked(player1, currentPosition1, direction, out Vector3 finalTarget1);
        bool isPlayer2Blocked = playerManagement.IsBlocked(player2, currentPosition2, direction, out Vector3 finalTarget2);

        // 1. Xác định đích đến THỰC TẾ (Nếu bị chặn thì đích đến chính là chỗ đang đứng)
        Vector3 actualTarget1 = isPlayer1Blocked ? currentPosition1 : finalTarget1;
        Vector3 actualTarget2 = isPlayer2Blocked ? currentPosition2 : finalTarget2;

        // 2. Kiểm tra khoảng cách dây xích dựa trên đích đến thực tế
        if (!IsDistanceAllowed(actualTarget1, actualTarget2))
        {
            // HIỆU ỨNG HAY: Nếu đi xa quá đứt xích, cả 2 sẽ bị giật nảy tại chỗ
            actualTarget1 = currentPosition1;
            actualTarget2 = currentPosition2;
            isPlayer1Blocked = true;
            isPlayer2Blocked = true;
        }

        // 3. Khóa điều khiển và gọi Coroutine cho cả 2 con (kèm theo cờ báo hiệu bị chặn)
        player1.isMoved = true;
        player2.isMoved = true;

        StartCoroutine(Move(player1, currentPosition1, direction, actualTarget1, isPlayer1Blocked));
        StartCoroutine(Move(player2, currentPosition2, direction, actualTarget2, isPlayer2Blocked));
    }


    // CHÚ Ý: Đã thêm tham số "bool isBlocked" vào cuối hàm
    IEnumerator Move(Player player, Vector3 currentPosition, Vector3 direction, Vector3 finalTarget, bool isBlocked)
    {
        float elapsedTime = 0f;
        Debug.Log("Đang gọi tiếng đi bộ!");
        AudioManager.instance.PlayMoving();

        while (elapsedTime < moveDuration)
        {
            // Lính gác chống lỗi bóng ma (Zombie Coroutine)
            if (player1 == null || player2 == null) yield break;

            float percent = elapsedTime / moveDuration;

            // Nếu bị chặn thì nhảy thấp hơn một chút (0.2f) cho tự nhiên, đi bình thường nhảy cao 0.5f
            float jumpHeight = isBlocked ? 0.2f : 0.5f;
            float heightOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;

            Vector3 currentPos;

            if (isBlocked)
            {
                // TẠO HIỆU ỨNG CỤNG TƯỜNG: 
                // Nhân vật rướn người về phía trước (hướng direction) tối đa 0.2 ô rồi lùi lại
                float bumpOffset = Mathf.Sin(percent * Mathf.PI) * 0.2f;
                currentPos = currentPosition + direction * bumpOffset;
            }
            else
            {
                // DI CHUYỂN BÌNH THƯỜNG: Trượt mượt mà đến ô tiếp theo (Bao gồm cả rớt vực, lên thang)
                currentPos = Vector3.Lerp(currentPosition, finalTarget, percent);
            }

            // Cộng thêm độ nảy trục Y
            currentPos.y += heightOffset;

            elapsedTime += Time.deltaTime;
            player.transform.position = currentPos;
            yield return null;
        }

        // Xử lý luật chơi (Chỉ xét rơi vực nếu KHÔNG bị chặn)
        if (!isBlocked && finalTarget.y <= -20f)
        {
            WinLoseManager.Instance.Lose();
        }

        // CHỐT TỌA ĐỘ: Đảm bảo nhân vật đứng chuẩn xác giữa ô vuông sau khi xong animation
        player.transform.position = isBlocked ? currentPosition : finalTarget;
        player.isMoved = false;

        var isWin = WinLoseManager.Instance.CheckWinCondition();
        var levelInplay = PlayerPrefs.GetInt("LevelInPlay");
        Debug.Log("Level IN play is" + levelInplay);
        if (isWin)
        {
            WinLoseManager.Instance.Win();
        }
    }


    bool IsDistanceAllowed(Vector3 finalTarget1, Vector3 finalTarget2)
    {

        if (finalTarget1 == null || finalTarget2 == null) return false;

        float distanceX = Mathf.Abs(finalTarget1.x - finalTarget2.x);
        float distanceZ = Mathf.Abs(finalTarget1.z - finalTarget2.z);


        if (distanceX > maxDistance + 0.1f || distanceZ > maxDistance + 0.1f)
        {
            toggleBanner.ShowWarning();

            return false;
        }

        return true;
    }

    public void SetupPlayer(Player player1, Player player2)
    {
        this.player1 = player1;
        this.player2 = player2;

        Debug.Log("Đã gán nhân vật, có thể di chuyển được rồi");
    }

    public void ResetMovement()
    {
        // 1. CHẶN ĐỨNG BÓNG MA: Dừng ngay lập tức Coroutine đang chạy dở
        StopAllCoroutines();

        // 3. Xé bỏ hồ sơ nhân viên cũ (Để Lính gác ở hàm Update chặn lại ngay)
        player1 = null;
        player2 = null;
    }
}
