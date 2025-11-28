using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour, IWeapon
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private WeaponInfo weaponInfo;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        MouseFollowWithOffset();
    }

    public void Attack()
    {
        Debug.Log("Book Attack");
    }
    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }


    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        // คำนวณมุมจากผู้เล่นไปยังเมาส์
        float angle = Mathf.Atan2(mousePos.y - playerScreenPoint.y, mousePos.x - playerScreenPoint.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            // หันซ้าย - ใช้ flipY แทนการหมุน
            ActiveWeapons.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = true;
            }
        }
        else
        {
            // หันขวา
            ActiveWeapons.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = false;
            }
        }
    }
}