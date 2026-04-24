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

    private Vector3 velocity;
    private Camera cam;
    private float fixedSize;

    void Start()
    {
        cam = GetComponent<Camera>();
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
        // Đo khoảng cách giữa 2 nhân vật
        float distanceBetweenTargets = Vector3.Distance(target1.position, target2.position);
        
        // Tính toán Size phù hợp
        float targetSize = Mathf.Lerp(minSize, maxSize, distanceBetweenTargets / distanceLimiter);
        
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
}