using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AreaExit exitDoor; // ประตูที่จะปลดล็อก
    [SerializeField] private bool lockDoorAtStart = true; // ล็อกประตูตอนเริ่ม

    private List<Enemy> enemies = new List<Enemy>();
    private bool allEnemiesDead = false;

    private void Start()
    {
        // หามอนสเตอร์ทั้งหมดใน Scene
        FindAllEnemies();

        // ล็อกประตูถ้าต้องการ
        if (lockDoorAtStart && exitDoor != null)
        {
            exitDoor.LockDoor();
            Debug.Log($"🔒 ล็อกประตู - เหลือมอนสเตอร์ {enemies.Count} ตัว");
        }
    }

    private void Update()
    {
        if (!allEnemiesDead)
        {
            CheckEnemies();
        }
    }

    private void FindAllEnemies()
    {
        // หามอนสเตอร์ทั้งหมดที่มี Enemy Component
        Enemy[] foundEnemies = FindObjectsOfType<Enemy>();
        enemies.AddRange(foundEnemies);

        Debug.Log($"🔍 พบมอนสเตอร์ทั้งหมด {enemies.Count} ตัว");
    }

    private void CheckEnemies()
    {
        // ลบมอนสเตอร์ที่ null (ตายแล้ว) ออกจาก List
        enemies.RemoveAll(enemy => enemy == null);

        // เช็คว่ามอนสเตอร์ตายหมดหรือยัง
        if (enemies.Count == 0)
        {
            OnAllEnemiesDead();
        }
    }

    private void OnAllEnemiesDead()
    {
        if (allEnemiesDead) return;

        allEnemiesDead = true;
        Debug.Log("💀 มอนสเตอร์ตายหมดแล้ว!");

        // ปลดล็อกประตู
        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
            Debug.Log("🚪 ปลดล็อกประตูแล้ว!");
        }
    }

    // ฟังก์ชันเพิ่มมอนสเตอร์ (ถ้ามี Spawner)
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"➕ เพิ่มมอนสเตอร์ - เหลือ {enemies.Count} ตัว");
        }
    }

    // ดูจำนวนมอนสเตอร์ที่เหลือ
    public int GetRemainingEnemyCount()
    {
        enemies.RemoveAll(enemy => enemy == null);
        return enemies.Count;
    }
}