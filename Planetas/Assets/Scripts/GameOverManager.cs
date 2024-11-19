using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;

    void Start()
    {
        int puntajeFinal = PlayerPrefs.GetInt("PlayerScore", 0);

        if (ScoreText != null)
        {
            ScoreText.text = "Score " + puntajeFinal.ToString();
            Debug.Log("Puntaje Final mostrado: " + puntajeFinal);
        }
        else
        {
            Debug.LogError("ScoreText no está asignado en GameOverManager.");
        }
    }

    public void ReiniciarJuego()
    {
        // Restablecer el tiempo del juego
        Time.timeScale = 1f;

        // Restablecer variables de juego solo si GameController está inicializado
        if (GameController.Instance != null)
        {
            GameController.Instance.puntos = 0;
            GameController.Instance.vidas = 3;
        }

        PlayerPrefs.SetInt("PlayerScore", 0);
        PlayerPrefs.SetInt("PlayerLives", 3);
        PlayerPrefs.Save();

        // Cargar la escena Level1
        SceneManager.LoadScene("Level1");
    }

    public void Menu()
    {
        // Restablecer el tiempo del juego
        Time.timeScale = 1f;

        // Restablecer variables de juego solo si GameController está inicializado
        if (GameController.Instance != null)
        {
            GameController.Instance.puntos = 0;
            GameController.Instance.vidas = 3;
        }

        PlayerPrefs.SetInt("PlayerScore", 0);
        PlayerPrefs.SetInt("PlayerLives", 3);
        PlayerPrefs.Save();

        // Cargar la escena MainMenu
        SceneManager.LoadScene("MainMenu");
    }

    public void MostrarGameOver()
    {
        // Iniciar corrutina para retrasar la carga de la escena de Game Over
        StartCoroutine(RetardoGameOver());
    }

    private IEnumerator RetardoGameOver()
    {
        // Esperar el tiempo necesario
        yield return new WaitForSeconds(0.01f);

        // Cargar la escena de Game Over
        SceneManager.LoadScene("GameOverScene");
    }
}
