using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    [Header("Nhân vật game")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;
    [Header("Layer game")]

    // Đã đổi tên để nhắc nhở: Tích chọn CẢ Ground và Stair ngoài Inspector vào ô này!
    [SerializeField] private LayerMask walkableLayer;

    [Header("Class cần thiết")]
    [SerializeField] private GameOver gameOver;

    [SerializeField] private Winzone Winzone;

    [SerializeField] private ToggleBanner toggleBanner;

    [SerializeField] private PlayerManagement playerManagement;

    [Header("Âm thanh")]
    [SerializeField] private AudioSource audioSource;

    // CHỐT KHÓA: Đừng quên cái này để tránh lỗi spam phím bay lên trời
    public float moveDuration = 0.2f;

    private bool isMoving = false;

    private bool isGameOver = false;

    [SerializeField] private int maxDistance;

    void Awake()
    {
        gameOver = FindAnyObjectByType<GameOver>(FindObjectsInactive.Include);
    }

    void Update()
    {

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

    void ProcessMoving(Player player1, Player player2, Vector3 direction)
    {
        var currentPosition1 = player1.transform.position;
        var currentPosition2 = player2.transform.position;

        // lay ra dc final target

        bool isPlayer1Blocked = playerManagement.IsBlocked(player1, currentPosition1, direction, out Vector3 finalTarget1);

        bool isPlayer2Blocked = playerManagement.IsBlocked(player2, currentPosition2, direction, out Vector3 finalTarget2);


        if (!isPlayer1Blocked)
        {
            player1.isMoved = true;
            if (!IsDistanceAllowed(finalTarget1, finalTarget2))
            {
                player1.isMoved = false;
                Debug.Log(finalTarget1);
                return;
            }
            StartCoroutine(Move(player1, currentPosition1, direction, finalTarget1));
        }
        if (!isPlayer2Blocked)
        {
            player2.isMoved = true;
            if (!IsDistanceAllowed(finalTarget1, finalTarget2))
            {
                player2.isMoved = false;
                Debug.Log(finalTarget2);
                return;
            }
            StartCoroutine(Move(player2, currentPosition2, direction, finalTarget2));
        }

    }

    IEnumerator Move(Player player, Vector3 currentPosition, Vector3 direction, Vector3 finalTarget)
    {
        float elapsedTime = 0f;
        Debug.Log("Đang gọi tiếng đi bộ!");
        AudioManager.instance.PlayMoving();
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
        if (finalTarget.y <= -20f)
        {
            gameOver.Lose();
        }
        player.transform.position = finalTarget;
        player.isMoved = false;
        Winzone.CheckWinCondition();
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
    void JumpSPX()
    {

    }
}
