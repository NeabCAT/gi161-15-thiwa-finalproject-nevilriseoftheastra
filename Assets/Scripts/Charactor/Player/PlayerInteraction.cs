using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;

    private PlayerControls playerControls;
    private AstraShard currentShard;

    void Awake()
    {
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        playerControls.Enable();
        playerControls.Interaction.Interact.performed += OnInteractPressed;
    }

    void OnDisable()
    {
        playerControls.Interaction.Interact.performed -= OnInteractPressed;
        playerControls.Disable();
    }

    void Update()
    {
        CheckForAstraShard();
    }

    void CheckForAstraShard()
    {
        // หา AstraShard ทั้งหมดในฉาก
        AstraShard[] allShards = FindObjectsOfType<AstraShard>();
        AstraShard nearestShard = null;
        float nearestDistance = float.MaxValue;

        foreach (AstraShard shard in allShards)
        {
            // ข้าม Shard ที่หยิบไปแล้ว
            if (!shard.CanInteract()) continue;

            float distance = Vector2.Distance(transform.position, shard.transform.position);

            if (distance <= interactionRange && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestShard = shard;
            }
        }

        // อัพเดท current shard
        if (currentShard != nearestShard)
        {
            // ออกจาก Shard เก่า
            if (currentShard != null)
            {
                currentShard.OnPlayerExit();
            }

            // เข้า Shard ใหม่
            currentShard = nearestShard;
            if (currentShard != null)
            {
                currentShard.OnPlayerEnter();
                Debug.Log($"💎 Found Astra Shard! Distance: {nearestDistance:F2}");
            }
        }
    }

    void OnInteractPressed(InputAction.CallbackContext context)
    {
        Debug.Log("🎮 E Button PRESSED!");

        if (currentShard != null && currentShard.CanInteract())
        {
            Debug.Log("✅ Picking up Astra Shard!");
            currentShard.OnInteractStart();
        }
        else
        {
            Debug.LogWarning("❌ No Astra Shard nearby!");
        }
    }

    void OnDrawGizmos()
    {
        // แสดงวงกลม Interaction Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // แสดงเส้นไปยัง Shard
        if (currentShard != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, currentShard.transform.position);
        }
    }

    public bool IsNearShard()
    {
        return currentShard != null;
    }

    public AstraShard GetCurrentShard()
    {
        return currentShard;
    }
}