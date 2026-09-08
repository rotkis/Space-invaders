using UnityEngine;

/// <summary>
/// Monta a formação inicial de naves comuns em uma grade (linhas x colunas).
/// Cada linha pode usar um prefab diferente (tipo 1, 2 ou 3), para variar
/// o sprite e a pontuação, como pedido no material (10/20/30 pontos).
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyRow
    {
        public GameObject enemyPrefab; // Prefab já com EnemyUnit configurado (sprite + pointValue).
        public int columns;
    }

    [SerializeField] private EnemyRow[] rows;
    [SerializeField] private Vector2 startPosition = new Vector2(-6f, 4f);
    [SerializeField] private float horizontalSpacing = 1.2f;
    [SerializeField] private float verticalSpacing = 0.8f;

    private void Start()
    {
        int totalCount = 0;
        Vector2 rowPos = startPosition;

        foreach (EnemyRow row in rows)
        {
            for (int col = 0; col < row.columns; col++)
            {
                Vector2 pos = new Vector2(rowPos.x + col * horizontalSpacing, rowPos.y);
                Instantiate(row.enemyPrefab, pos, Quaternion.identity, transform);
                totalCount++;
            }
            rowPos.y -= verticalSpacing;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterEnemyCount(totalCount);
        }
    }
}
