using UnityEngine;

public abstract class ItemInteract : MonoBehaviour
{
    [Header("Item Base Settings")]
    public string itemType = "DefaultItem";
    public string itemName = "Item";
    [TextArea(2, 4)]
    public string itemDescription = "";

    // Method สำหรับเช็คว่าสามารถ interact ได้หรือไม่
    public virtual bool CanInteract()
    {
        return true;
    }

    // เมื่อผู้เล่นเข้าใกล้
    public virtual void OnPlayerEnter()
    {
        Debug.Log($"[{itemType}] ผู้เล่นเข้าใกล้");
    }

    // เมื่อผู้เล่นออกจากระยะ
    public virtual void OnPlayerExit()
    {
        Debug.Log($"[{itemType}] ผู้เล่นออกจากระยะ");
    }

    // เมื่อเริ่มกด interact
    public virtual void OnInteractStart()
    {
        Debug.Log($"[{itemType}] เริ่ม Interact");
    }

    // เมื่อปล่อย interact
    public virtual void OnInteractEnd()
    {
        Debug.Log($"[{itemType}] ปล่อย Interact");
    }

    // สำหรับ interact แบบกดค้าง (deltaTime = เวลาที่กดค้าง)
    public virtual void OnInteractHold(float deltaTime)
    {
        // ใช้สำหรับ item ที่ต้องกดค้าง
    }

    // เมื่อหยิบ item
    public virtual void OnPickup(Player player)
    {
        Debug.Log($"[{itemType}] หยิบโดย {player.name}");
    }

    public virtual void ApplyEffect()
    {
        Debug.Log($"[{itemType}] ใช้งาน Effect");
    }

    public virtual void DestroyItem()
    {
        Destroy(gameObject);
    }
}