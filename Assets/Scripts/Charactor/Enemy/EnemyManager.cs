using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AreaExit exitDoor;
    [SerializeField] private bool lockDoorAtStart = true;
    [SerializeField] private bool autoFindDoor = true;
    [SerializeField] private string doorTag = "ExitDoor";

    private List<Enemy> enemies = new List<Enemy>();
    private bool allEnemiesDead = false;

    private string currentScene = "";

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene โหลด: {scene.name}");
        ResetManager();
        Initialize();
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        Initialize();
    }

    private void Initialize()
    {
        if (exitDoor == null && autoFindDoor)
        {
            if (!string.IsNullOrEmpty(doorTag))
            {
                GameObject doorObj = GameObject.FindGameObjectWithTag(doorTag);
                if (doorObj != null)
                {
                    exitDoor = doorObj.GetComponent<AreaExit>();
                    Debug.Log($"หาประตูจาก Tag [{doorTag}] เจอ: {exitDoor?.name}");
                }
            }

            if (exitDoor == null)
            {
                exitDoor = FindObjectOfType<AreaExit>();
                if (exitDoor != null)
                {
                    Debug.Log($"หาประตูอัตโนมัติเจอ: {exitDoor.name}");
                }
                else
                {
                    Debug.LogWarning("ไม่พบประตู (AreaExit) ในซีน!");
                }
            }
        }

        FindAllEnemies();

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

    private void ResetManager()
    {
        enemies.Clear();
        allEnemiesDead = false; 
        exitDoor = null; 
        Debug.Log("EnemyManager รีเซ็ตแล้ว");
    }

    private void FindAllEnemies()
    {
        Enemy[] foundEnemies = FindObjectsOfType<Enemy>();
        enemies.Clear();
        enemies.AddRange(foundEnemies);

        Debug.Log($"พบมอนสเตอร์ทั้งหมด {enemies.Count} ตัว");
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
            Debug.Log($"กำลังปลดล็อกประตู: {exitDoor.name}");
            exitDoor.UnlockDoor();
            Debug.Log($"สถานะประตูหลังปลดล็อก: IsLocked = {exitDoor.IsLocked()}");
        }
        else
        {
            Debug.LogError(" Exit Door เป็น NULL!");
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"เพิ่มมอนสเตอร์ - เหลือ {enemies.Count} ตัว");
        }
    }

    public int GetRemainingEnemyCount()
    {
        enemies.RemoveAll(enemy => enemy == null);
        return enemies.Count;
    }

    public void ManualReset()
    {
        Debug.Log("ManualReset ถูกเรียก");
        ResetManager();
        Initialize();
    }
}