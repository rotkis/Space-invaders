using System.Collections;
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
    private int currentLives = 3;

    [Header("Pontuação")]
    private int score = 0;

    [Header("Dificuldade / Velocidade")]
    [Tooltip("Multiplicador aplicado à velocidade das naves comuns. Aumenta a cada nave destruída.")]
    [SerializeField] private float baseEnemySpeedMultiplier = 1f;
    [SerializeField] private float speedIncrementPerKill = 0.03f;
    public float EnemySpeedMultiplier { get; private set; }

    [Header("Efeitos")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float playerHitFreezeDuration = 3f;

    [Header("Cenas")]
    [SerializeField] private string victorySceneName = "VictoryScene";
    [SerializeField] private string defeatSceneName = "DefeatScene";

    private int totalEnemiesAlive = 0;
    private bool gameEnded = false;
    private bool hitFreezeActive = false;
    private PlayerController playerController;

    // Chaves usadas para levar o placar final para as cenas de fim de jogo.
    public const string PlayerPrefsScoreKey = "FinalScore";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetRuntimeState();
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    public void ResetForNewGame()
    {
        ResetRuntimeState();
        playerController = FindAnyObjectByType<PlayerController>();
    }

    private void ResetRuntimeState()
    {
        currentLives = startingLives;
        score = 0;
        EnemySpeedMultiplier = baseEnemySpeedMultiplier;
        totalEnemiesAlive = 0;
        gameEnded = false;
        hitFreezeActive = false;
        Time.timeScale = 1f;
    }

    public void SpawnExplosion(Vector3 position)
    {
        if (explosionPrefab == null)
            return;

        Instantiate(explosionPrefab, position, Quaternion.identity);
    }

    public void ClearProjectiles()
    {
        Missile[] projectiles = FindObjectsByType<Missile>(FindObjectsSortMode.None);
        foreach (Missile missile in projectiles)
        {
            if (missile != null)
            {
                Destroy(missile.gameObject);
            }
        }
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
        EnemySpeedMultiplier += speedIncrementPerKill;

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

    public void PlayerHit()
    {
        if (gameEnded || hitFreezeActive)
            return;

        StartCoroutine(HandlePlayerHit());
    }

    private IEnumerator HandlePlayerHit()
    {
        hitFreezeActive = true;
        ClearProjectiles();

        if (playerController != null)
        {
            playerController.TriggerHitFlash();
            SpawnExplosion(playerController.transform.position);
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(playerHitFreezeDuration);
        Time.timeScale = 1f;

        currentLives--;

        if (currentLives <= 0)
        {
            if (playerController != null)
            {
                SpawnExplosion(playerController.transform.position);
            }

            Defeat();
            yield break;
        }

        hitFreezeActive = false;

        if (playerController != null)
        {
            playerController.ResetAfterHit();
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
