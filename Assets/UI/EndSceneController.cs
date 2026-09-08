using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Usado tanto na cena de Vitória quanto na de Derrota.
/// Mostra o placar final (salvo em PlayerPrefs pelo GameManager) e
/// permite reiniciar o jogo.
/// </summary>
public class EndSceneController : MonoBehaviour
{
    [SerializeField] private Text finalScoreText;
    [SerializeField] private string gameSceneName = "MainScene";

    private void Start()
    {
        int finalScore = PlayerPrefs.GetInt(GameManager.PlayerPrefsScoreKey, 0);
        if (finalScoreText != null)
        {
            finalScoreText.text = "PONTUAÇÃO FINAL: " + finalScore;
        }
    }

    /// <summary>Ligue este método ao OnClick de um botão "Jogar Novamente".</summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
