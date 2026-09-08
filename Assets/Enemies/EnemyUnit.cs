using UnityEngine;

/// <summary>
/// Comportamento de uma nave inimiga comum, seguindo exatamente a lógica
/// descrita no material da aula:
///   1) Incrementa X por 10 passos
///   2) Decrementa X por 10 passos
///   3) Decrementa 5 unidades de Y
///   4) Repete do passo 1
/// Além disso atira mísseis aleatoriamente e termina o jogo se tocar a
/// parede inferior.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyUnit : MonoBehaviour
{
    private enum State { MovingRight, MovingLeft }

    [Header("Pontuação")]
    [Tooltip("Pontos ganhos ao destruir esta nave (10 / 20 / 30 conforme o tipo).")]
    [SerializeField] private int pointValue = 10;

    [Header("Movimento (em 'passos')")]
    [SerializeField] private int stepsPerDirection = 10;
    [SerializeField] private float stepSize = 0.25f;      // Unidades por passo em X
    [SerializeField] private float dropAmount = 0.5f;      // Unidades descidas em Y (equivalente às "5 unidades")
    [SerializeField] private float stepInterval = 0.4f;    // Tempo entre passos (afetado pela velocidade global)

    [Header("Limite / Derrota")]
    [SerializeField] private float bottomLimitY = -4.5f;

    [Header("Tiro")]
    [SerializeField] private GameObject enemyMissilePrefab;
    [SerializeField] private float shootChancePerSecond = 0.15f;

    private Rigidbody2D rb2d;
    private State currentState = State.MovingRight;
    private int stepsTaken = 0;
    private float stepTimer = 0f;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleStepMovement();
        HandleRandomShooting();

        if (transform.position.y <= bottomLimitY)
        {
            GameManager.Instance.EnemyReachedBottom();
        }
    }

    private void HandleStepMovement()
    {
        float speedMultiplier = GameManager.Instance != null ? GameManager.Instance.EnemySpeedMultiplier : 1f;
        stepTimer += Time.deltaTime * speedMultiplier;

        if (stepTimer < stepInterval) return;
        stepTimer = 0f;

        Vector2 pos = rb2d.position;

        if (currentState == State.MovingRight)
        {
            pos.x += stepSize;
        }
        else
        {
            pos.x -= stepSize;
        }

        stepsTaken++;

        if (stepsTaken >= stepsPerDirection)
        {
            stepsTaken = 0;

            if (currentState == State.MovingRight)
            {
                currentState = State.MovingLeft;
            }
            else
            {
                currentState = State.MovingRight;
                // Só desce quando completa o ciclo direita->esquerda (passo 3 do enunciado).
                pos.y -= dropAmount;
            }
        }

        rb2d.MovePosition(pos);
    }

    private void HandleRandomShooting()
    {
        if (enemyMissilePrefab == null) return;

        // Chance por segundo convertida em chance por frame.
        if (Random.value < shootChancePerSecond * Time.deltaTime)
        {
            Instantiate(enemyMissilePrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerMissile"))
        {
            GameManager.Instance.OnEnemyDestroyed(pointValue);
            Destroy(gameObject);
        }
    }
}
