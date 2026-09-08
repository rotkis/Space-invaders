using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton central do jogo. Controla vidas, pontuação, velocidade dos
/// inimigos (que aumenta conforme eles são destruídos) e o carregamento
/// das cenas de vitória/derrota.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Vidas")]
    [SerializeField] private int startingLives = 3;
    private int currentLives;

    [Header("Pontuação")]
    private int score = 0;

    [Header("Dificuldade / Velocidade")]
    [Tooltip("Multiplicador aplicado à velocidade das naves comuns. Aumenta a cada nave destruída.")]
    [SerializeField] private float baseEnemySpeedMultiplier = 1f;
    [SerializeField] private float speedIncrementPerKill = 0.03f;
    public float EnemySpeedMultiplier { get; private set; }

    [Header("Cenas")]
    [SerializeField] private string victorySceneName = "VictoryScene";
    [SerializeField] private string defeatSceneName = "DefeatScene";

    private int totalEnemiesAlive = 0;
    private bool gameEnded = false;

    // Chaves usadas para levar o placar final para as cenas de fim de jogo.
    public const string PlayerPrefsScoreKey = "FinalScore";

    private void Awake()
    {
        // Garante uma única instância entre carregamentos de cena.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentLives = startingLives;
        EnemySpeedMultiplier = baseEnemySpeedMultiplier;
    }

    /// <summary>Chamado pelo EnemySpawner assim que a leva de inimigos é criada.</summary>
    public void RegisterEnemyCount(int count)
    {
        totalEnemiesAlive = count;
    }

    /// <summary>Chamado por uma EnemyUnit ao ser destruída por um míssil do jogador.</summary>
    public void OnEnemyDestroyed(int pointValue)
    {
        if (gameEnded) return;

        AddScore(pointValue);

        totalEnemiesAlive--;
        EnemySpeedMultiplier += speedIncrementPerKill; // Quanto menos naves, mais rápido o jogo fica.

        if (totalEnemiesAlive <= 0)
        {
            Victory();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    /// <summary>Chamado pela MotherShip ao ser destruída. Só soma pontos:
    /// a nave chefe não faz parte da leva fixa contada para a vitória.</summary>
    public void OnMotherShipDestroyed(int pointValue)
    {
        AddScore(pointValue);
    }

    public int GetScore() => score;
    public int GetLives() => currentLives;

    /// <summary>Chamado pelo PlayerController quando o jogador é atingido.</summary>
    public void PlayerHit()
    {
        if (gameEnded) return;

        currentLives--;
        if (currentLives <= 0)
        {
            Defeat();
        }
    }

    /// <summary>Chamado por uma EnemyUnit quando encosta na parede/base inferior.</summary>
    public void EnemyReachedBottom()
    {
        Defeat();
    }

    private void Victory()
    {
        if (gameEnded) return;
        gameEnded = true;
        PlayerPrefs.SetInt(PlayerPrefsScoreKey, score);
        SceneManager.LoadScene(victorySceneName);
    }

    private void Defeat()
    {
        if (gameEnded) return;
        gameEnded = true;
        PlayerPrefs.SetInt(PlayerPrefsScoreKey, score);
        SceneManager.LoadScene(defeatSceneName);
    }
}
