using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] monsterPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int monstersToSpawn = 5;
    [SerializeField] private float spawnDelay = 0.5f;

    [Header("Trigger Settings")]
    [SerializeField] private bool requirePlayerTrigger = true;
    [SerializeField] private bool spawnOnce = true;

    [Header("Optional Settings")]
    [SerializeField] private bool randomMonster = true;
    [SerializeField] private GameObject spawnEffect;

    [Header("Auto Destroy Settings")]
    [SerializeField] private bool destroyWhenAllDead = true;

    // ⭐ NEW: อ้างอิงถึงประตูที่จะเปิด
    [Header("Door Control")]
    [SerializeField] private AreaExit exitDoor;

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (requirePlayerTrigger && other.GetComponent<PlayerController>())
        {
            if (spawnOnce && hasSpawned)
            {
                return;
            }

            StartSpawning();
        }
    }

    private void Update()
    {
        if (destroyWhenAllDead && hasSpawned)
        {
            CleanUpDeadMonsters();

            if (spawnedMonsters.Count == 0)
            {
                OnAllMonstersDead();
            }
        }
    }

    public void StartSpawning()
    {
        if (spawnOnce && hasSpawned)
        {
            Debug.Log("⚠️ Spawner นี้ Spawn ไปแล้ว!");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    public void SpawnAllNow()
    {
        if (spawnOnce && hasSpawned)
        {
            Debug.Log("⚠️ Spawner นี้ Spawn ไปแล้ว!");
            return;
        }

        for (int i = 0; i < monstersToSpawn; i++)
        {
            SpawnMonster();
        }

        hasSpawned = true;
    }

    private IEnumerator SpawnRoutine()
    {
        Debug.Log($"👹 [{gameObject.name}] เริ่ม Spawn มอนสเตอร์ {monstersToSpawn} ตัว!");

        for (int i = 0; i < monstersToSpawn; i++)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnDelay);
        }

        hasSpawned = true;
        Debug.Log($"✅ [{gameObject.name}] Spawn มอนสเตอร์เสร็จแล้ว!");
    }

    private void SpawnMonster()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            Debug.LogError("❌ ไม่มี Monster Prefab!");
            return;
        }

        GameObject monsterPrefab;
        if (randomMonster)
        {
            monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
        }
        else
        {
            monsterPrefab = monsterPrefabs[0];
        }

        Vector3 spawnPosition = GetSpawnPosition();

        if (spawnEffect != null)
        {
            Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
        }

        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        spawnedMonsters.Add(monster);

        Debug.Log($"👹 Spawn มอนสเตอร์ที่ {spawnPosition}");
    }

    private Vector3 GetSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return spawnPoint.position;
        }
        else
        {
            return transform.position;
        }
    }

    public void CleanUpDeadMonsters()
    {
        spawnedMonsters.RemoveAll(monster => monster == null);
    }

    public void DestroyAllMonsters()
    {
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        spawnedMonsters.Clear();
        Debug.Log("💀 ทำลายมอนสเตอร์ทั้งหมด!");
    }

    public void ResetSpawner()
    {
        hasSpawned = false;
        Debug.Log("🔄 รีเซ็ต Spawner แล้ว!");
    }

    public bool AllMonstersDead()
    {
        CleanUpDeadMonsters();
        return spawnedMonsters.Count == 0;
    }

    // ⭐ เมื่อมอนสเตอร์ตายหมด -> เปิดประตูและทำลาย Spawner
    private void OnAllMonstersDead()
    {
        Debug.Log($"💀 [{gameObject.name}] มอนสเตอร์ตายหมดแล้ว!");

        // ⭐ เปิดประตูทางออก
        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
            Debug.Log("🚪 เปิดประตูทางออกแล้ว!");
        }

        // ทำลาย Spawner
        Debug.Log($"💥 ทำลาย Spawner...");
        Destroy(gameObject);
    }

    public int CurrentMonsterCount
    {
        get
        {
            CleanUpDeadMonsters();
            return spawnedMonsters.Count;
        }
    }

    public bool HasSpawned => hasSpawned;

    private void OnDrawGizmos()
    {
        if (requirePlayerTrigger)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                Gizmos.DrawCube(transform.position, boxCollider.size);
            }
        }

        Gizmos.color = Color.red;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                    Gizmos.DrawLine(transform.position, point.position);
                }
            }
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}