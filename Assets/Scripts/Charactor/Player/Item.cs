using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    public string itemType;
    public int value;
    public Sprite sprite;

    public void OnPickup(Player player)
    {
        Debug.Log($"Player picked up {itemType}");
        ApplyEffect();
        Destroy(gameObject);
    }

    public void ApplyEffect()
    {
        Debug.Log($"Applied effect: {itemType} with value {value}");
    }
}