using UnityEngine;

/// <summary>
/// Script genérico de míssil, usado tanto pelo jogador quanto pelas naves
/// inimigas. A diferença entre os dois é apenas a Tag do prefab
/// ("PlayerMissile" ou "EnemyMissile") e o sentido do movimento.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Missile : MonoBehaviour
{
    public enum Owner { Player, Enemy }

    [SerializeField] private Owner owner = Owner.Player;
    [SerializeField] private float speed = 8f;

    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Míssil do jogador sobe (Y positivo), míssil inimigo desce (Y negativo).
        float direction = owner == Owner.Player ? 1f : -1f;
        rb2d.velocity = new Vector2(0f, direction * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner == Owner.Player)
        {
            // Míssil do jogador atinge nave comum ou nave mãe.
            if (other.CompareTag("Enemy") || other.CompareTag("MotherShip"))
            {
                // As próprias EnemyUnit/MotherShip cuidam de avisar o GameManager
                // e se destruir; aqui só removemos o míssil.
                Destroy(gameObject);
            }
            else if (other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
        else // Owner.Enemy
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.PlayerHit();
                Destroy(gameObject);
            }
            else if (other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }
}
