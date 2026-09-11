using UnityEngine;

/// <summary>
/// Nave chefe (mother ship). Conforme o material:
///   1) Aparece no canto superior esquerdo em intervalos aleatórios (30-50s, controlado pelo Spawner)
///   2) Move +5 unidades em X a cada passo
///   3) Desaparece ao chegar no canto superior direito
/// Vale 50 pontos se destruída pelo jogador.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MotherShip : MonoBehaviour
{
    [SerializeField] private int pointValue = 50;
    [SerializeField] private float stepSize = 0.5f;
    [SerializeField] private float stepInterval = 0.1f;
    [SerializeField] private float rightEdgeX = 9f;

    private Rigidbody2D rb2d;
    private float stepTimer = 0f;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        stepTimer += Time.deltaTime;
        if (stepTimer < stepInterval) return;
        stepTimer = 0f;

        Vector2 pos = rb2d.position;
        pos.x += stepSize;
        rb2d.MovePosition(pos);

        if (pos.x >= rightEdgeX)
        {
            Destroy(gameObject); // Chegou ao canto oposto sem ser destruída.
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerMissile"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SpawnExplosion(transform.position);
                GameManager.Instance.OnMotherShipDestroyed(pointValue);
            }

            Destroy(gameObject);
        }
    }
}
