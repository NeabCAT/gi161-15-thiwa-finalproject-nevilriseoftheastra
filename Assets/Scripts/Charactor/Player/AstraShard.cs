using UnityEngine;

public enum ClassType
{
    None,
    Striker,
    Arcanist,
    AstraCharm
}

public class AstraShard : ItemInteract
{
    [Header("Astra Shard Settings")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private ClassType[] availableClasses = { ClassType.Striker, ClassType.Arcanist, ClassType.AstraCharm };

    [Header("Visual Effect")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmount = 0.3f;

    private Vector3 startPosition;
    private Player playerReference;

    void Start()
    {
        itemType = "AstraShard";
        startPosition = transform.position;

        ResetShard();
    }

    void Update()
    {
        if (!isActivated && spriteRenderer != null)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }

    public void ResetShard()
    {
        isActivated = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        Debug.Log(" AstraShard Reset แล้ว - สามารถหยิบได้อีกครั้ง");
    }

    public override bool CanInteract()
    {
        return !isActivated;
    }

    public override void OnPlayerEnter()
    {
        if (!isActivated)
        {
            Debug.Log(" [AstraShard] เข้าใกล้ Astra Shard - กด E เพื่อหยิบ");
        }
    }

    public override void OnPlayerExit()
    {
        Debug.Log(" [AstraShard] ห่างจาก Astra Shard");
    }

    public override void OnInteractStart()
    {
        if (isActivated) return;

        playerReference = FindObjectOfType<Player>();
        if (playerReference != null)
        {
            OnPickup(playerReference);
        }
    }

    public override void OnInteractEnd() { }

    public override void OnInteractHold(float deltaTime) { }

    public override void OnPickup(Player player)
    {
        isActivated = true;
        playerReference = player;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        Debug.Log(" [AstraShard] หยิบ Astra Shard สำเร็จ!");
        ShowClassSelection();
    }

    public override void ApplyEffect()
    {
        Debug.Log(" [AstraShard] ApplyEffect");
    }

    public void ShowClassSelection()
    {
        ClassSelectionUI ui = FindObjectOfType<ClassSelectionUI>();
        if (ui != null)
        {
            ui.ShowSelection(this, availableClasses);
        }
    }

    public void GrantPower(Player player, ClassType selectedClass)
    {
        if (player != null)
        {
            player.SelectClass(selectedClass);
            Debug.Log($"✨ [AstraShard] มอบพลังคลาส {selectedClass} ให้ผู้เล่น");
        }
    }
}