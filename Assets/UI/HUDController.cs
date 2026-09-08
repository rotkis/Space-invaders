using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Atualiza os textos de pontuação e vidas na cena do jogo.
/// Coloque este script em um GameObject de Canvas e arraste os
/// componentes Text correspondentes no Inspector.
/// </summary>
public class HUDController : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null)
            scoreText.text = "SCORE: " + GameManager.Instance.GetScore();

        if (livesText != null)
            livesText.text = "VIDAS: " + GameManager.Instance.GetLives();
    }
}
