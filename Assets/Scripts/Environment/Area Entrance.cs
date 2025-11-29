using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;

    private void Start()
    {
        if (transitionName == SceneManagement.Instance.SceneTransitionName)
        {
            // ⭐ เปลี่ยนจาก Start เป็น Coroutine
            StartCoroutine(SetupEntranceRoutine());
        }
    }

    // ⭐ เพิ่ม Coroutine ใหม่
    private IEnumerator SetupEntranceRoutine()
    {
        // 1. ตั้งตำแหน่ง Player ทันที
        PlayerController.Instance.transform.position = transform.position;

        // 2. บังคับให้ Camera Update ทันที
        if (CameraController.Instance != null)
        {
            CameraController.Instance.SetPlayerCameraFollow();

            // ⭐ บังคับให้ Camera Snap ไปที่ตำแหน่ง Player ทันที
            Camera.main.transform.position = new Vector3(
                PlayerController.Instance.transform.position.x,
                PlayerController.Instance.transform.position.y,
                Camera.main.transform.position.z
            );
        }

        // 3. รอ 1 frame ให้ระบบทุกอย่าง Update
        yield return null;

        // 4. ค่อย Fade เข้า
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToClear();
        }
    }
}