using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlayer : MonoBehaviour
{
    [Header("Nhân vật game")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    void Start()
    {
        PlayerMoving playerMoving = FindObjectOfType<PlayerMoving>();
        playerMoving.SetupPlayer(player1, player2);

        IsometricGroupCamera camera = FindObjectOfType<IsometricGroupCamera>();
        camera.SetupPlayer(player1, player2);
    }
}
