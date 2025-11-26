using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxOffset = -0.15f;
    [SerializeField] private bool smoothMovement = false;
    [SerializeField] private float smoothSpeed = 10f;

    private Camera cam;
    private Vector3 startPos;
    private Vector3 lastCamPos;

    private void Awake()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            enabled = false;
        }
    }

    private void Start()
    {
        startPos = transform.position;
        lastCamPos = cam.transform.position;
    }

    private void LateUpdate()
    {
        // เช็คว่ากล้องเคลื่อนที่จริงๆ หรือไม่
        if (cam.transform.position == lastCamPos)
            return;

        Vector3 travel = cam.transform.position - startPos;
        Vector3 targetPos = startPos + travel * parallaxOffset;

        if (smoothMovement)
        {
            // เคลื่อนที่แบบนุ่มนวล
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // เคลื่อนที่แบบทันที
            transform.position = targetPos;
        }

        lastCamPos = cam.transform.position;
    }
}