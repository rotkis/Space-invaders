using System.Collections;
using UnityEngine;

/// <summary>
/// Controla a nave do jogador: movimento horizontal, disparo de mísseis
/// e limites da tela. A perda de vida é resolvida pelo Missile (inimigo)
/// chamando GameManager.PlayerHit(); aqui só tratamos colisão direta com
/// uma nave inimiga (ex: uma nave que desceu até o jogador).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minX = -8.5f;
    [SerializeField] private float maxX = 8.5f;

    [Header("Tiro")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.5f;
    private float fireTimer = 0f;

    [Header("Hit / Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashInterval = 0.08f;
    [SerializeField] private int flashCount = 8;

    private Rigidbody2D rb2d;
    private Color originalColor;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float input = Input.GetAxisRaw("Horizontal"); // Setas ou A/D
        Vector2 pos = rb2d.position;
        pos.x += input * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        rb2d.MovePosition(pos);
    }

    private void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            if (fireTimer <= 0f)
            {
                Fire();
                fireTimer = fireCooldown;
            }
        }
    }

    private void Fire()
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Instantiate(missilePrefab, spawnPos, Quaternion.identity);
    }

    public void TriggerHitFlash()
    {
        if (spriteRenderer == null)
            return;

        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    public void ResetAfterHit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.enabled = true;
        }
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSecondsRealtime(flashInterval);
        }

        spriteRenderer.enabled = true;
        spriteRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("MotherShip"))
        {
            GameManager.Instance.PlayerHit();
        }
    }
}
