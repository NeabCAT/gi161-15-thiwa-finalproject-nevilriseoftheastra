using System.Collections;
using UnityEngine;

public class MagicLaser : MonoBehaviour
{
    [SerializeField] private float laserGrowTime = 2f;

    private bool isGrowing = true;
    private float laserRange;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider2D;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        // ⭐ บังคับใช้ Tiled Mode เพื่อไม่ให้ Sprite ถูก Stretch
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.tileMode = SpriteTileMode.Continuous;

        // ตั้งค่าสีให้มี Glow (ถ้าต้องการ)
        // spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    private void Start()
    {
        LaserFaceMouse();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Indestructible>() && !other.isTrigger)
        {
            isGrowing = false;
        }
    }

    public void UpdateLaserRange(float laserRange)
    {
        this.laserRange = laserRange;
        StartCoroutine(IncreaseLaserLengthRoutine());
    }

    private IEnumerator IncreaseLaserLengthRoutine()
    {
        float timePassed = 0f;

        while (spriteRenderer.size.x < laserRange && isGrowing)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / laserGrowTime;
            float currentLength = Mathf.Lerp(1f, laserRange, linearT);

            // ⭐ Sprite Renderer (Tiled Mode จะทำให้ไม่เหลี่ยม)
            spriteRenderer.size = new Vector2(currentLength, 1f);

            // ⭐ Collider (ขยายตาม)
            capsuleCollider2D.size = new Vector2(currentLength, capsuleCollider2D.size.y);
            capsuleCollider2D.offset = new Vector2(currentLength / 2, capsuleCollider2D.offset.y);

            yield return null;
        }

        // เริ่ม Fade
        SpriteFade spriteFade = GetComponent<SpriteFade>();
        if (spriteFade != null)
        {
            StartCoroutine(spriteFade.SlowFadeRoutine());
        }
    }

    private void LaserFaceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = transform.position - mousePosition;
        transform.right = -direction;
    }
}