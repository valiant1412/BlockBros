using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    [Header("Cấu hình")]
    [SerializeField] private int playerIndex;
    [Tooltip("Nếu để trống, script sẽ dùng tất cả Renderer bên trong Player.")]
    [SerializeField] private Renderer[] playerRenderers;
    [SerializeField] private SkinDatabaseSO skinDatabase;

    private void Awake()
    {
        if (playerRenderers == null || playerRenderers.Length == 0)
        {
            playerRenderers = GetComponentsInChildren<Renderer>(true);
        }
    }

    private void Start()
    {
        LoadAndApplyPlayerSkin();
    }

    private void OnEnable()
    {
        SkinShopManager.OnSkinChanged += ApplySkin;
    }
    private void OnDisable()
    {
        SkinShopManager.OnSkinChanged -= ApplySkin;
    }

    private void LoadAndApplyPlayerSkin()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.gameData == null || skinDatabase == null) return;

        string currentSkin = SaveManager.Instance.gameData.currentSkin[playerIndex];
        ApplySkinByID(currentSkin);
    }

    private void ApplySkin(int targetPlayerIndex, string skinID)
    {
        if (targetPlayerIndex != playerIndex) return;
        ApplySkinByID(skinID);
    }

    private void ApplySkinByID(string skinID)
    {
        if (skinDatabase == null) return;

        if (!skinDatabase.TryGetMaterial(skinID, playerIndex, out Material material))
        {
            Debug.LogWarning($"Không tìm thấy material skin '{skinID}' cho Player {playerIndex + 1}.", this);
            return;
        }

        foreach (Renderer renderer in playerRenderers)
        {
            if (renderer == null) continue;

            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            renderer.sharedMaterials = materials;
        }
    }
}
