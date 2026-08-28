using UnityEngine;

public class IsometricGroupCamera : MonoBehaviour
{
    [Header("Mục tiêu")]
    public Transform target1;
    public Transform target2;

    [Header("Góc nhìn & Vị trí (Tâm hoàn hảo)")]
    [Tooltip("Khoảng cách từ Camera soi thẳng xuống nhân vật (Ví dụ: 20)")]
    public float distance = 20f;

    [Tooltip("Độ mượt khi lướt camera (0.15 - 0.3 là đẹp)")]
    public float moveSmoothTime = 0.2f;

    [Header("Tinh chỉnh khung hình")]
    [Tooltip("Dùng trục X, Y để đẩy nhân vật lệch khỏi tâm (chừa chỗ cho UI nếu cần)")]
    public Vector3 framingOffset = new Vector3(0f, 0f, 0f);

    [Header("Cài đặt Zoom (Tự động)")]
    public bool enableAutoZoom = false;
    public float minSize = 4f;
    public float maxSize = 12f;
    public float distanceLimiter = 15f;
    public float zoomSmoothSpeed = 5f;
    [Tooltip("Khoảng trống quanh hai nhân vật, tính theo world units.")]
    [SerializeField] private float viewportPadding = 1.25f;

    private Vector3 velocity;
    private Camera cam;
    private float fixedSize;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Stylized Water đọc depth texture để tạo chuyển màu theo độ sâu và bọt ở mép nước.
        // Dùng toán tử |= để không ghi đè các DepthTextureMode khác nếu hiệu ứng khác đã bật chúng.
        cam.depthTextureMode |= DepthTextureMode.Depth;

        cam.orthographic = true;
        fixedSize = cam.orthographicSize;

        // Đưa camera về đúng tâm ngay khi bắt đầu (không bị giật khung hình đầu tiên)
        SnapToCenter();
    }

    void LateUpdate()
    {
        if (target1 == null || target2 == null) return;

        MoveCamera();

        if (enableAutoZoom)
        {
            ZoomCamera();
        }
        else
        {
            // Nếu tắt Auto Zoom, camera giữ nguyên kích thước Size cài đặt ngoài Inspector
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, fixedSize, Time.deltaTime * zoomSmoothSpeed);
        }
    }

    void MoveCamera()
    {
        // 1. Tìm điểm chính giữa 2 nhân vật (bao gồm cả trục Y)
        Vector3 centerPoint = (target1.position + target2.position) / 2f;

        // 2. TÍNH TOÁN TOÁN HỌC: 
        // Từ điểm giữa -> lùi lại theo hướng Camera đang nhìn (transform.forward) một đoạn bằng 'distance'
        Vector3 targetPosition = centerPoint - (transform.forward * distance) + framingOffset;

        // 3. Lướt mượt mà đến vị trí đó
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveSmoothTime);
    }

    void ZoomCamera()
    {
        // Orthographic size is half of the visible height. Calculate in camera space
        // so portrait and landscape screens frame the same gameplay area correctly.
        Vector3 target1InView = transform.InverseTransformPoint(target1.position);
        Vector3 target2InView = transform.InverseTransformPoint(target2.position);
        float halfHeight = Mathf.Abs(target1InView.y - target2InView.y) * 0.5f + viewportPadding;
        float halfWidth = Mathf.Abs(target1InView.x - target2InView.x) * 0.5f + viewportPadding;
        float targetSize = Mathf.Max(halfHeight, halfWidth / cam.aspect);
        targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

        // Zoom mượt mà
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSmoothSpeed);
    }

    // Hàm hỗ trợ đưa camera về tâm ngay lập tức (không delay)
    public void SnapToCenter()
    {
        if (target1 == null || target2 == null) return;
        Vector3 centerPoint = (target1.position + target2.position) / 2f;
        transform.position = centerPoint - (transform.forward * distance) + framingOffset;
    }
    public void SetupPlayer(Player player1, Player player2)
    {
        target1 = player1.transform;
        target2 = player2.transform;

        SnapToCenter();
        Debug.Log("Đã gán nhân vật, camera có thể theo nhân vật được rồi");
    }
}
