using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineCamera cinemachineCamera;
    public void SetPlayerCameraFollow()
    {
        cinemachineCamera = FindObjectOfType<CinemachineCamera>();
        cinemachineCamera.Follow = PlayerController.Instance.transform;
    }
}
