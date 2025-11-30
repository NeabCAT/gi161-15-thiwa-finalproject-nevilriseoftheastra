using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ⭐ เพิ่ม

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AreaExit exitDoor;
    [SerializeField] private bool lockDoorAtStart = true;
    [SerializeField] private bool autoFindDoor = true;
    [SerializeField] private string doorTag = "ExitDoor";

    private List<Enemy> enemies = new List<Enemy>();
    private bool allEnemiesDead = false;

    // ⭐ เพิ่มตัวแปรเก็บ Scene ปัจจุบัน
    private string currentScene = "";

    private void Awake()
    {
        // ⭐ Subscribe Event เมื่อ Scene โหลด
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // ⭐ Unsubscribe เมื่อ Object ถูกทำลาย
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// ⭐ ฟังก์ชันนี้จะทำงานอัตโนมัติทุกครั้งที่ Scene โหลด
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🔄 Scene โหลด: {scene.name}");

        // Reset Manager ทุกครั้งที่โหลด Scene
        ResetManager();
        Initialize();
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        Initialize();
    }

    /// <summary>
    /// ⭐ ฟังก์ชัน Initialize แยกออกมาจาก Start
    /// </summary>
    private void Initialize()
    {
        // หาประตูอัตโนมัติถ้าไม่ได้ลาก
        if (exitDoor == null && autoFindDoor)
        {
            // ลองหาจาก Tag ก่อน
            if (!string.IsNullOrEmpty(doorTag))
            {
                GameObject doorObj = GameObject.FindGameObjectWithTag(doorTag);
                if (doorObj != null)
                {
                    exitDoor = doorObj.GetComponent<AreaExit>();
                    Debug.Log($"✅ หาประตูจาก Tag [{doorTag}] เจอ: {exitDoor?.name}");
                }
            }

            // ถ้ายังไม่เจอ ให้หาแบบธรรมดา
            if (exitDoor == null)
            {
                exitDoor = FindObjectOfType<AreaExit>();
                if (exitDoor != null)
                {
                    Debug.Log($"✅ หาประตูอัตโนมัติเจอ: {exitDoor.name}");
                }
                else
                {
                    Debug.LogWarning("⚠️ ไม่พบประตู (AreaExit) ในซีน!");
                }
            }
        }

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

    /// <summary>
    /// ⭐ รีเซ็ต Manager ทุกครั้งที่โหลดซีน (แก้ไขแล้ว)
    /// </summary>
    private void ResetManager()
    {
        enemies.Clear();
        allEnemiesDead = false; // ⭐ สำคัญมาก!
        exitDoor = null; // ⭐ เคลียร์ประตูเก่า
        Debug.Log("🔄 EnemyManager รีเซ็ตแล้ว");
    }

    private void FindAllEnemies()
    {
        Enemy[] foundEnemies = FindObjectsOfType<Enemy>();
        enemies.Clear();
        enemies.AddRange(foundEnemies);

        Debug.Log($"🔍 พบมอนสเตอร์ทั้งหมด {enemies.Count} ตัว");
    }

    private void CheckEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null);

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

        if (exitDoor != null)
        {
            Debug.Log($"🔓 กำลังปลดล็อกประตู: {exitDoor.name}");
            exitDoor.UnlockDoor();
            Debug.Log($"✅ สถานะประตูหลังปลดล็อก: IsLocked = {exitDoor.IsLocked()}");
        }
        else
        {
            Debug.LogError("❌ Exit Door เป็น NULL!");
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"➕ เพิ่มมอนสเตอร์ - เหลือ {enemies.Count} ตัว");
        }
    }

    public int GetRemainingEnemyCount()
    {
        enemies.RemoveAll(enemy => enemy == null);
        return enemies.Count;
    }

    /// <summary>
    /// ⭐ รีเซ็ต Manual (ไม่จำเป็นแล้ว แต่เก็บไว้สำหรับเรียกจากที่อื่น)
    /// </summary>
    public void ManualReset()
    {
        Debug.Log("🔧 ManualReset ถูกเรียก");
        ResetManager();
        Initialize();
    }
}