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

    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se uma nave inimiga encostar diretamente no jogador.
        if (other.CompareTag("Enemy") || other.CompareTag("MotherShip"))
        {
            GameManager.Instance.PlayerHit();
        }
    }
}
