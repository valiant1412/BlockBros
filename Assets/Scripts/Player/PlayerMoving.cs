using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public float rotateSpeed = 2f;

    [SerializeField] private int maxDistance;

    [Header("Điều khiển vuốt")]
    [SerializeField, Min(10f)] private float minSwipeDistance = 35f;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private float inputLockedUntil;
    private bool isInputEnabled;
    private bool isTrackingSwipe;
    // Lưu một lượt vuốt kế tiếp trong lúc hai nhân vật đang chạy để thao tác nhanh không bị mất.
    private bool hasBufferedMove;
    private Vector3 bufferedMoveDirection;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isInputEnabled || Time.unscaledTime < inputLockedUntil) return;

        // Vẫn ghi nhận swipe khi nhân vật đang di chuyển; hướng đó sẽ được chạy ngay khi cả hai đứng lại.
        HandleSwipeInput();

        if (!CanAcceptInput()) return;

        if (hasBufferedMove)
        {
            Vector3 direction = bufferedMoveDirection;
            ClearBufferedMove();
            ProcessMoving(player1, player2, direction);
            return;
        }

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

    private void HandleSwipeInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Chỉ chặn thao tác bắt đầu trên UI; nhả tay trên UI không được làm mất swipe đang theo dõi.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                isTrackingSwipe = false;
                return;
            }

            startTouchPosition = Input.mousePosition;
            isTrackingSwipe = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isTrackingSwipe) return;

            isTrackingSwipe = false;
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        if (swipeDelta.magnitude >= minSwipeDistance)
        {
            // Normalize để đưa vector về độ dài bằng 1, giúp so sánh chính xác
            swipeDelta.Normalize();

            // 1. Định nghĩa 4 hướng vuốt chéo trên màn hình 2D
            Vector2 upRight = new Vector2(1, 1).normalized;   // ↗
            Vector2 upLeft = new Vector2(-1, 1).normalized;  // ↖
            Vector2 downRight = new Vector2(1, -1).normalized;  // ↘
            Vector2 downLeft = new Vector2(-1, -1).normalized; // ↙

            // 2. Chấm điểm xem đường vuốt của người chơi giống hướng nào nhất
            float scoreUR = Vector2.Dot(swipeDelta, upRight);
            float scoreUL = Vector2.Dot(swipeDelta, upLeft);
            float scoreDR = Vector2.Dot(swipeDelta, downRight);
            float scoreDL = Vector2.Dot(swipeDelta, downLeft);

            // 3. Tìm ra hướng có điểm số cao nhất (Người chơi muốn vuốt hướng đó)
            float maxScore = Mathf.Max(scoreUR, Mathf.Max(scoreUL, Mathf.Max(scoreDR, scoreDL)));

            // 4. Lập bản đồ (Mapping) từ 2D sang 3D
            // LƯU Ý: Bạn có thể cần tráo đổi các Vector3.forward/back/left/right 
            // ở dưới đây sao cho khớp với trục X, Z trong Scene của bạn!

            if (maxScore == scoreUR) // Vuốt ↗ (Mũi tên đỏ của bạn)
            {
                RequestMove(Vector3.right); // Hoặc Vector3.right
            }
            else if (maxScore == scoreUL) // Vuốt ↖
            {
                RequestMove(Vector3.forward);    // Hoặc Vector3.forward
            }
            else if (maxScore == scoreDL) // Vuốt ↙
            {
                RequestMove(Vector3.left);    // Hoặc Vector3.left
            }
            else if (maxScore == scoreDR) // Vuốt ↘
            {
                RequestMove(Vector3.back);   // Hoặc Vector3.back
            }
        }
    }

    private void RequestMove(Vector3 direction)
    {
        if (player1 == null || player2 == null) return;

        if (CanAcceptInput())
        {
            ProcessMoving(player1, player2, direction);
            return;
        }

        // Chỉ giữ một lệnh kế tiếp để game phản hồi nhanh nhưng không biến swipe spam thành hàng đợi dài.
        if (!hasBufferedMove)
        {
            bufferedMoveDirection = direction;
            hasBufferedMove = true;
        }
    }
    void ProcessMoving(Player player1, Player player2, Vector3 direction)
    {
        if (!CanAcceptInput()) return;

        bool isPlayer1Active = player1.gameObject.activeInHierarchy;
        bool isPlayer2Active = player2.gameObject.activeInHierarchy;
        var currentPosition1 = player1.transform.position;
        var currentPosition2 = player2.transform.position;

        Vector3 finalTarget1 = currentPosition1;
        Vector3 finalTarget2 = currentPosition2;
        bool isPlayer1Blocked = !isPlayer1Active || playerManagement.IsBlocked(player1, currentPosition1, direction, out finalTarget1);
        bool isPlayer2Blocked = !isPlayer2Active || playerManagement.IsBlocked(player2, currentPosition2, direction, out finalTarget2);

        ResolvePlayerDestinationCollision(
            player1,
            currentPosition1,
            ref finalTarget1,
            ref isPlayer1Blocked,
            isPlayer1Active,
            player2,
            currentPosition2,
            ref finalTarget2,
            ref isPlayer2Blocked,
            isPlayer2Active);

        // 1. Xác định đích đến THỰC TẾ (Nếu bị chặn thì đích đến chính là chỗ đang đứng)
        Vector3 actualTarget1 = isPlayer1Blocked ? currentPosition1 : finalTarget1;
        Vector3 actualTarget2 = isPlayer2Blocked ? currentPosition2 : finalTarget2;

        // 2. Kiểm tra khoảng cách dây xích dựa trên đích đến thực tế
        if (isPlayer1Active && isPlayer2Active && !IsDistanceAllowed(actualTarget1, actualTarget2))
        {
            // HIỆU ỨNG HAY: Nếu đi xa quá đứt xích, cả 2 sẽ bị giật nảy tại chỗ
            actualTarget1 = currentPosition1;
            actualTarget2 = currentPosition2;
            isPlayer1Blocked = true;
            isPlayer2Blocked = true;
        }

        // 3. Khóa điều khiển và gọi Coroutine cho cả 2 con (kèm theo cờ báo hiệu bị chặn)
        if (isPlayer1Active)
        {
            player1.SetState(PlayerState.Moving);
            StartCoroutine(Move(player1, currentPosition1, direction, actualTarget1, isPlayer1Blocked));
        }

        if (isPlayer2Active)
        {
            player2.SetState(PlayerState.Moving);
            StartCoroutine(Move(player2, currentPosition2, direction, actualTarget2, isPlayer2Blocked));
        }
    }

    private static void ResolvePlayerDestinationCollision(
        Player player1,
        Vector3 currentPosition1,
        ref Vector3 finalTarget1,
        ref bool isPlayer1Blocked,
        bool isPlayer1Active,
        Player player2,
        Vector3 currentPosition2,
        ref Vector3 finalTarget2,
        ref bool isPlayer2Blocked,
        bool isPlayer2Active)
    {
        if (!isPlayer1Active || !isPlayer2Active ||
            !WillCollidersOverlap(player1, finalTarget1, player2, finalTarget2))
        {
            return;
        }

        bool player1WillMove = !isPlayer1Blocked &&
            (finalTarget1 - currentPosition1).sqrMagnitude > 0.0001f;
        bool player2WillMove = !isPlayer2Blocked &&
            (finalTarget2 - currentPosition2).sqrMagnitude > 0.0001f;

        if (player1WillMove && !player2WillMove)
        {
            isPlayer1Blocked = true;
            finalTarget1 = currentPosition1;
        }
        else if (player2WillMove && !player1WillMove)
        {
            isPlayer2Blocked = true;
            finalTarget2 = currentPosition2;
        }
        else
        {
            // Hai nhân vật cùng nhắm vào một thể tích hoặc đã chồng lên nhau.
            // Chặn cả hai để không tạo thêm một lần dính collider mới.
            isPlayer1Blocked = true;
            isPlayer2Blocked = true;
            finalTarget1 = currentPosition1;
            finalTarget2 = currentPosition2;
        }

        HapticManager.HeavyTaptic();
    }

    private static bool WillCollidersOverlap(
        Player firstPlayer,
        Vector3 firstPosition,
        Player secondPlayer,
        Vector3 secondPosition)
    {
        Collider firstCollider = firstPlayer.GetComponentInChildren<Collider>();
        Collider secondCollider = secondPlayer.GetComponentInChildren<Collider>();
        if (firstCollider == null || secondCollider == null) return false;

        Bounds firstBounds = firstCollider.bounds;
        firstBounds.center += firstPosition - firstPlayer.transform.position;
        firstBounds.extents = new Vector3(
            Mathf.Max(0f, firstBounds.extents.x - 0.02f),
            firstBounds.extents.y,
            Mathf.Max(0f, firstBounds.extents.z - 0.02f));

        Bounds secondBounds = secondCollider.bounds;
        secondBounds.center += secondPosition - secondPlayer.transform.position;
        secondBounds.extents = new Vector3(
            Mathf.Max(0f, secondBounds.extents.x - 0.02f),
            secondBounds.extents.y,
            Mathf.Max(0f, secondBounds.extents.z - 0.02f));

        return firstBounds.Intersects(secondBounds);
    }


    // CHÚ Ý: Đã thêm tham số "bool isBlocked" vào cuối hàm
    IEnumerator Move(Player player, Vector3 currentPosition, Vector3 direction, Vector3 finalTarget, bool isBlocked)
    {
        float elapsedTime = 0f;
        Debug.Log("Đang gọi tiếng đi bộ!");
        player.FaceDirection(direction);
        AudioManager.instance.PlayMoving();
        // Ví dụ trong hàm nhận diện Input của bạn:
        // Move(Vector2.up);
        HapticManager.LightTaptic();
        while (elapsedTime < moveDuration)
        {
            // Lính gác chống lỗi bóng ma (Zombie Coroutine)
            if (player == null || !player.gameObject.activeInHierarchy) yield break;

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

        // Thêm code để tránh việc đụng tường gây lệch.
        // (Tránh lệch do Time.deltaTime không đều khiến percent != 1.0 chính xác)
        player.transform.position = isBlocked ? currentPosition : finalTarget;
        // Xử lý luật chơi (Chỉ xét rơi vực nếu KHÔNG bị chặn)
        if (!isBlocked && finalTarget.y <= -20f)
        {
            WinLoseManager.Instance.Lose();
            yield break;
        }

        if (player.gameObject.activeInHierarchy &&
            (WinLoseManager.Instance == null || !WinLoseManager.Instance.isGameEnded))
        {
            player.SetState(PlayerState.Stand);
        }
    }

    bool IsDistanceAllowed(Vector3 finalTarget1, Vector3 finalTarget2)
    {

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

    }

    public void ResetMovement()
    {
        StopMovement();
        player1 = null;
        player2 = null;
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        isTrackingSwipe = false;
        startTouchPosition = Vector2.zero;
        endTouchPosition = Vector2.zero;
        ClearBufferedMove();
    }

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
        isTrackingSwipe = false;
        startTouchPosition = Vector2.zero;
        endTouchPosition = Vector2.zero;
        ClearBufferedMove();
    }

    private bool CanAcceptInput()
    {
        if (player1 == null || player2 == null) return false;

        bool player1Ready = !player1.gameObject.activeInHierarchy || player1.currentState == PlayerState.Stand;
        bool player2Ready = !player2.gameObject.activeInHierarchy || player2.currentState == PlayerState.Stand;
        return player1Ready && player2Ready &&
            (player1.gameObject.activeInHierarchy || player2.gameObject.activeInHierarchy);
    }

    public void LockInputFor(float seconds)
    {
        inputLockedUntil = Time.unscaledTime + seconds;
        isTrackingSwipe = false;
        startTouchPosition = Vector2.zero;
        endTouchPosition = Vector2.zero;
        ClearBufferedMove();
    }

    private void ClearBufferedMove()
    {
        hasBufferedMove = false;
        bufferedMoveDirection = Vector3.zero;
    }
}
