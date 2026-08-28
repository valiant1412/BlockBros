using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isBlockByObject = false;

    public bool isBlockByPlayer = false;

    public bool isMoved { get; set; }

    public bool isReachedMaxDistance = false;

    public Vector3 targetPosition { get; set; }

    [SerializeField] private LayerMask blockLayer;

    public PlayerState currentState;

    private RaycastHit hit;
    [SerializeField] private Transform visualRoot;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    public float minSwipeDistance = 50f;

    [SerializeField] private PlayerManager playerManagement;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trap"))
        {
            WinLoseManager.Instance.Lose();
        }
    }
    // Start is called before the first frame update
    public bool CheckBlockedByObject(Vector3 currentPosition, Vector3 direction, LayerMask blockLayer)
    {
        float bottomY = currentPosition.y;
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
        {
            bottomY = col.bounds.min.y;
        }

        Vector3 origin = new Vector3(currentPosition.x, bottomY + 0.25f, currentPosition.z);
        Debug.DrawRay(origin, direction * 1f, Color.red, 2f);

        if (Physics.Raycast(origin, direction, out hit, 1f, blockLayer))
        {
            Debug.DrawRay(hit.point, hit.normal * 2f, Color.black, 2f);
            // Stair colliders have vertical risers. They are walkable surfaces, so a
            // horizontal ray hitting a riser must not be treated as a wall.
            bool isStair = hit.collider.CompareTag("Stair");
            if ((!isStair && Mathf.Abs(hit.normal.y) < 0.01f) || hit.collider.CompareTag("Trap"))
            {
                isBlockByObject = true;
                return isBlockByObject;
            }
        }

        isBlockByObject = false;
        return isBlockByObject;
    }
    public bool CheckBlockedByPlayer(Vector3 currentPosition, Vector3 direction, LayerMask playerLayer)
    {
        isBlockByPlayer = false;
        if (!Physics.Raycast(currentPosition, direction, out hit, 1f, playerLayer)) return false;

        Player player = hit.collider.GetComponentInParent<Player>();
        if (player == null || !player.gameObject.activeInHierarchy || player.currentState == PlayerState.Exit)
        {
            return false;
        }

        // Đã thấy một nhân vật đang đứng ở hướng đi thì đó là vật cản trực tiếp.
        // Việc nhân vật đó có đang đứng trước tường hay không không làm thay đổi
        // việc ô của họ đang bị chiếm.
        isBlockByPlayer = true;
        return isBlockByPlayer;
    }
    public void SetState(PlayerState newState)
    {
        currentState = newState;
    }
    public void FaceDirection(Vector3 direction)
    {
        if (visualRoot == null || direction.sqrMagnitude <= 0f) return;

        visualRoot.rotation = Quaternion.LookRotation(direction);
    }

    public void ResetForSpawn(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        gameObject.SetActive(true);
        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        isBlockByObject = false;
        isBlockByPlayer = false;
        isMoved = false;
        isReachedMaxDistance = false;
        targetPosition = spawnPoint.position;
        currentState = PlayerState.Stand;

        if (visualRoot != null)
        {
            visualRoot.localRotation = Quaternion.identity;
        }

        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }
}
