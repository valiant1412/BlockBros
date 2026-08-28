using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Nhân vật game")]
    [SerializeField] private Player player1Prefab;
    [SerializeField] private Player player2Prefab;

    private Player player1;

    private Player player2;
    public bool IsInstantiate { get; private set; }
    // Cho UI biết input đang được bật do gameplay hay đang bị tắt bởi một popup/menu.
    public bool IsGameplayActive { get; private set; }
    private LevelContext currentLevelContext;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    public void SetupPlayer(LevelContext levelContext)
    {
        if (levelContext == null || !levelContext.HasValidSpawnPoints)
        {
            Debug.LogError("LevelContext is missing Player1Spawn or Player2Spawn.");
            return;
        }

        currentLevelContext = levelContext;
        if (!IsInstantiate)
        {
            player1 = Instantiate(player1Prefab, levelContext.player1Spawn.position, levelContext.player1Spawn.rotation);
            player2 = Instantiate(player2Prefab, levelContext.player2Spawn.position, levelContext.player2Spawn.rotation);
            IsInstantiate = true;

        }
        else
        {
            RespawnPlayers();
        }

        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.SetupPlayer(player1, player2);
            PlayerMoving.Instance.SetInputEnabled(false);
            PlayerMoving.Instance.LockInputFor(0.2f);
        }

        IsometricGroupCamera camera = FindObjectOfType<IsometricGroupCamera>();
        if (camera != null)
        {
            camera.SetupPlayer(player1, player2);
        }
    }

    public void RespawnPlayers()
    {
        if (!IsInstantiate || currentLevelContext == null || !currentLevelContext.HasValidSpawnPoints) return;

        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.StopMovement();
        }

        player1.ResetForSpawn(currentLevelContext.player1Spawn);
        player2.ResetForSpawn(currentLevelContext.player2Spawn);

        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.SetupPlayer(player1, player2);
            PlayerMoving.Instance.LockInputFor(0.2f);
        }

        IsometricGroupCamera camera = FindObjectOfType<IsometricGroupCamera>();
        if (camera != null) camera.SetupPlayer(player1, player2);
    }

    public void SetGameplayActive(bool isActive)
    {
        IsGameplayActive = isActive;

        if (PlayerMoving.Instance != null)
        {
            PlayerMoving.Instance.SetInputEnabled(isActive);
            if (isActive) PlayerMoving.Instance.LockInputFor(0.15f);
        }
    }
}
