using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TransparentDetection : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float transparencyAmount = 0.8f;
    [SerializeField] private float fadeTime = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;
    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();

        if (spriteRenderer == null && tilemap == null)
        {
            Debug.LogError($"GameObject '{gameObject.name}' ต้องมี SpriteRenderer หรือ Tilemap component!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.activeInHierarchy) return;

        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            if (spriteRenderer != null)
            {
                currentFadeCoroutine = StartCoroutine(FadeRoutine(spriteRenderer, fadeTime, spriteRenderer.color.a, transparencyAmount));
            }
            else if (tilemap != null)
            {
                currentFadeCoroutine = StartCoroutine(FadeRoutine(tilemap, fadeTime, tilemap.color.a, transparencyAmount));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!gameObject.activeInHierarchy) return;

        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            if (spriteRenderer != null)
            {
                currentFadeCoroutine = StartCoroutine(FadeRoutine(spriteRenderer, fadeTime, spriteRenderer.color.a, 1f));
            }
            else if (tilemap != null)
            {
                currentFadeCoroutine = StartCoroutine(FadeRoutine(tilemap, fadeTime, tilemap.color.a, 1f));
            }
        }
    }

    private IEnumerator FadeRoutine(SpriteRenderer sr, float duration, float startValue, float targetTransparency)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // เช็คว่า SpriteRenderer ยังมีอยู่หรือไม่
            if (sr == null) yield break;

            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, targetTransparency, elapsedTime / duration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);

            yield return null;
        }

        // ตั้งค่าสุดท้ายให้แน่นอน
        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, targetTransparency);
        }
    }

    private IEnumerator FadeRoutine(Tilemap tm, float duration, float startValue, float targetTransparency)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // เช็คว่า Tilemap ยังมีอยู่หรือไม่
            if (tm == null) yield break;

            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, targetTransparency, elapsedTime / duration);
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, newAlpha);

            yield return null;
        }

        // ตั้งค่าสุดท้ายให้แน่นอน
        if (tm != null)
        {
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, targetTransparency);
        }
    }

    private void OnDisable()
    {
        // หยุด Coroutine เมื่อ GameObject ถูกปิด
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }
    }
}