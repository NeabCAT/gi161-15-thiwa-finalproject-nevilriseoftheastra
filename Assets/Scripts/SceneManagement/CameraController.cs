using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : Singleton<CameraController>
{
    private CinemachineCamera cinemachineCamera;

    protected override void Awake()
    {
        base.Awake();

        // ⭐ ฟัง Event เมื่อโหลดซีนใหม่
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // ยกเลิกการฟัง Event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SetPlayerCameraFollow();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"📸 ซีนโหลดเสร็จ: {scene.name} - กำลังหา Player ใหม่...");

        // รอ 1 frame แล้วค่อยหา Player
        StartCoroutine(WaitAndSetPlayer());
    }

    private IEnumerator WaitAndSetPlayer()
    {
        // รอ 2 frames ให้ Player พร้อม
        yield return null;
        yield return null;

        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow()
    {
        // หา Cinemachine Camera ใหม่ทุกครั้ง
        if (cinemachineCamera == null)
        {
            cinemachineCamera = FindObjectOfType<CinemachineCamera>();
        }

        if (cinemachineCamera == null)
        {
            Debug.LogError("❌ ไม่พบ CinemachineCamera ในซีน!");
            return;
        }

        // ⭐ หา Player ใหม่ทุกครั้ง
        if (PlayerController.Instance != null)
        {
            cinemachineCamera.Follow = PlayerController.Instance.transform;
            Debug.Log($"✅ กล้องตาม Player แล้ว: {PlayerController.Instance.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ ไม่พบ PlayerController.Instance - ลองหาด้วย Tag");

            // ⭐ ลองหาด้วย Tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                cinemachineCamera.Follow = playerObj.transform;
                Debug.Log($"✅ กล้องตาม Player (จาก Tag) แล้ว: {playerObj.name}");
            }
            else
            {
                Debug.LogError("❌ ไม่พบ Player ด้วย Tag 'Player'!");
            }
        }
    }

    public void RefreshCamera()
    {
        cinemachineCamera = null; // รีเซ็ตเพื่อหาใหม่
        SetPlayerCameraFollow();
    }
}