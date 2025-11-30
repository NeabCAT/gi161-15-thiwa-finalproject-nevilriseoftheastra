using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Summon Settings")]
    [SerializeField] private GameObject minionPrefab; // ลูกน้องที่จะเสก
    [SerializeField] private int minionCount = 3; // จำนวนลูกน้องที่เสกต่อครั้ง
    [SerializeField] private float summonRadius = 3f; // รัศมีการเสกรอบตัว
    [SerializeField] private float summonCooldown = 8f; // คูลดาวน์การเสก
    [SerializeField] private Color summonColor = Color.magenta; // สีตอนเสก
    [SerializeField] private float summonDuration = 1.5f; // ระยะเวลาเสก

    [Header("Summon VFX")]
    [SerializeField] private GameObject summonCirclePrefab; // วงแดงบอกตำแหน่ง spawn
    [SerializeField] private float circleDisplayTime = 1f; // เวลาแสดงวงก่อน spawn

    [Header("Victory UI")]
    [SerializeField] private BossVictoryUI victoryUI;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool canSummon = true;
    private bool isSummoning = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        if (!isDead)
        {
            if (!isSummoning && canSummon && PlayerController.Instance != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

                if (distanceToPlayer < attackRange)
                {
                    StartCoroutine(SummonMinionsRoutine());
                }
            }

            if (!isSummoning)
            {
                MovementStateControl();
            }
        }
    }

    private IEnumerator SummonMinionsRoutine()
    {
        canSummon = false;
        isSummoning = true;

        Debug.Log($"🔮 [{gameObject.name}] เริ่มเสกลูกน้อง!");

        // หยุดเคลื่อนที่ขณะเสก
        if (enemyPathfinding != null)
        {
            enemyPathfinding.StopMoving();
        }

        // เปลี่ยนสีเป็นสีเสก
        if (spriteRenderer != null)
        {
            spriteRenderer.color = summonColor;
        }

        // เล่น Animation เสก
        if (animator != null)
        {
            animator.SetTrigger("Summon");
        }

        // สุ่มตำแหน่งที่จะ spawn ลูกน้อง
        Vector2[] spawnPositions = new Vector2[minionCount];
        GameObject[] spawnCircles = new GameObject[minionCount];

        for (int i = 0; i < minionCount; i++)
        {
            // สุ่มตำแหน่งรอบๆ Boss
            float angle = (360f / minionCount) * i + Random.Range(-20f, 20f);
            float distance = Random.Range(summonRadius * 0.5f, summonRadius);

            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * distance;

            spawnPositions[i] = (Vector2)transform.position + offset;

            // สร้างวงแดงบอกตำแหน่ง
            if (summonCirclePrefab != null)
            {
                spawnCircles[i] = Instantiate(summonCirclePrefab, spawnPositions[i], Quaternion.identity);
            }
        }

        // รอให้แสดงวงก่อน
        yield return new WaitForSeconds(circleDisplayTime);

        // Spawn ลูกน้องทุกตัว
        for (int i = 0; i < minionCount; i++)
        {
            if (minionPrefab != null)
            {
                GameObject minion = Instantiate(minionPrefab, spawnPositions[i], Quaternion.identity);
                Debug.Log($"👾 Spawn ลูกน้อง: {minion.name} ที่ตำแหน่ง {spawnPositions[i]}");
            }

            // ลบวงแดง
            if (spawnCircles[i] != null)
            {
                Destroy(spawnCircles[i]);
            }
        }

        // รอให้การเสกเสร็จ
        yield return new WaitForSeconds(summonDuration - circleDisplayTime);

        // เปลี่ยนสีกลับ
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isSummoning = false;

        // เริ่มนับคูลดาวน์
        StartCoroutine(SummonCooldownRoutine());
    }

    private IEnumerator SummonCooldownRoutine()
    {
        yield return new WaitForSeconds(summonCooldown);
        canSummon = true;
        Debug.Log($"✅ [{gameObject.name}] พร้อมเสกอีกครั้ง!");
    }

    public override void IsDead()
    {
        isSummoning = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        StartCoroutine(ShowVictoryAfterDeath());
        HandleBossDeath();
    }

    private void HandleBossDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"💀 [{gameObject.name}] ตาย!");

        // ปิดการเคลื่อนที่
        if (enemyPathfinding != null)
        {
            enemyPathfinding.StopMoving();
            enemyPathfinding.enabled = false;
        }

        // ปิด Collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // เล่น Death Animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

    }


    private IEnumerator ShowVictoryAfterDeath()
    {
        Debug.Log("🎬 ShowVictoryAfterDeath เริ่มต้น");

        // ฆ่า Enemy ทุกตัวในฉาก
        KillAllEnemies();

        // เล่น Victory Animation ของ Player
        PlayPlayerVictoryAnimation();

        // รอให้ Death Animation เล่นเสร็จ
        float deathAnimationTime = 1.5f;
        Debug.Log($"รอ {deathAnimationTime} วินาที...");
        yield return new WaitForSeconds(deathAnimationTime);

        Debug.Log("กำลังเรียก ShowVictory...");

        // แสดง Victory UI
        if (victoryUI != null)
        {
            Debug.Log("Victory UI พบแล้ว! เรียก ShowVictory()");
            victoryUI.ShowVictory(gameObject.name);
        }
        else
        {
            Debug.LogError("Victory UI เป็น NULL! ลืมลาก Reference ใน Inspector?");
        }

        // รอให้ UI แสดงผล
        yield return new WaitForSeconds(0.5f);

        // ตอนนี้ถึงค่อย Destroy Boss
        Debug.Log("Destroy Boss GameObject");
        Destroy(gameObject);
    }

    private void KillAllEnemies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != this && enemy != null && enemy.IsAlive())
            {
                enemy.IsDead();
                Debug.Log($"💀 ฆ่า {enemy.gameObject.name}");
            }
        }

        Debug.Log($"🔥 ฆ่า Enemy ทั้งหมด {allEnemies.Length - 1} ตัว!");
    }


    private void PlayPlayerVictoryAnimation()
    {
        if (Player.Instance != null)
        {
            Animator playerAnimator = Player.Instance.GetComponent<Animator>();

            if (playerAnimator != null)
            {
                // เล่น Victory Animation
                playerAnimator.SetTrigger("Victory");
                Debug.Log("🎉 Player Victory Animation!");
            }
            else
            {
                Debug.LogWarning("⚠️ Player ไม่มี Animator!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ ไม่พบ Player.Instance!");
        }
    }
}