using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] protected string itemType;
    [SerializeField] protected int value;
    [SerializeField] protected Sprite sprite;

    public abstract void OnPickup(Player player);
    public abstract void ApplyEffect();
}