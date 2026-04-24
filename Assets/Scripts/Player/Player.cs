using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    public bool isBlockByObject = false;

    public bool isBlockByPlayer = false;

    public bool isMoved {get; set;}

    public bool isReachedMaxDistance = false;

    public Vector3 targetPosition {get; set;}
    [SerializeField] private LayerMask blockLayer;

    private  RaycastHit hit;

    [SerializeField] private GameOver gameOver;

    void Awake()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trap"))
        {
            gameOver.Lose();
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
            if (Mathf.Abs(hit.normal.y) < 0.01f || hit.collider.CompareTag("Trap")) 
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
        var isBlockByPlayer = Physics.Raycast(currentPosition, direction,out hit, 1f,playerLayer);
        if (isBlockByPlayer)
        {   
            var player = hit.collider.gameObject.GetComponent<Player>();

            if (player != null)
            {   
                var isBesidePlayerBlocking = player.CheckBlockedByObject(player.transform.position,direction,blockLayer);
                if (isBesidePlayerBlocking)
                {   
                    this.isBlockByPlayer = isBlockByPlayer;
                    return isBlockByPlayer;
                }
                
            }
        }
        else
        {
            this.isBlockByPlayer = isBlockByPlayer;
        }
        return this.isBlockByPlayer;
    }
}
