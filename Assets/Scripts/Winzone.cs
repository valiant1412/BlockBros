using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Winzone : MonoBehaviour
{
    // Start is called before the first frame update
    private float playerArrived = 0;
    [Header("Nhân vật chơi")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Winzone")]
    [SerializeField] private Transform winzone1;
    [SerializeField] private Transform winzone2;
    [SerializeField]private GameOver gameOverManager;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckWinCondition()
    {
        Vector3 player1Position = new Vector3(player1.transform.position.x, 0f, player1.transform.position.z);
        Vector3 player2Position = new Vector3(player2.transform.position.x, 0f, player2.transform.position.z);

        // position of winzone
        Vector3 winzone1Position = new Vector3(winzone1.transform.position.x, 0f, winzone1.transform.position.z);
        Vector3 winzone2Position = new Vector3(winzone2.transform.position.x, 0f, winzone2.transform.position.z);

        //check distance
        bool p1_is_on_zone1 = Vector3.Distance(player1Position , winzone1Position)<0.1f;
        bool p2_is_on_zone2 = Vector3.Distance(player2Position , winzone2Position)<0.1f;

        bool p1_is_on_zone2 = Vector3.Distance(player1Position , winzone2Position)<0.1f;
        bool p2_is_on_zone1 = Vector3.Distance(player2Position , winzone1Position)<0.1f;

        if(( p1_is_on_zone1 && p2_is_on_zone2) || (p1_is_on_zone2 && p2_is_on_zone1))
        {
            gameOverManager.Win();
        }
    }
}
