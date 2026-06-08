using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlayer : MonoBehaviour
{
    [Header("Nhân vật game")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [SerializeField] private Transform winzone1;

    [SerializeField] private Transform winzone2;
    void Start()
    {
        //assign cho camera
        IsometricGroupCamera camera = FindObjectOfType<IsometricGroupCamera>();
        camera.SetupPlayer(player1, player2);

        //assign cho Player Moving
        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.SetupPlayer(player1, player2);
        }
        // assign cho Player Management.
        if (WinLoseManagement.Instance != null)
        {
            WinLoseManagement.Instance.SetupInput(player1, player2, winzone1, winzone2);
        }
    }
}
