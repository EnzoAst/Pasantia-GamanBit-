using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCongratulationsManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText; // Asignar desde el inspector el UI para el texto de puntaje

    void Start()
    {
        // Obtener el puntaje final desde PlayerPrefs
        int puntajeFinal = PlayerPrefs.GetInt("PlayerScore", 0);

        // Mostrar el puntaje en el UI
        ScoreText.text = "Score " + puntajeFinal;

        Debug.Log("Puntaje Final mostrado en Congratulations: " + puntajeFinal);
    }

    public void ReiniciarJuego()
    {
        GameController.Instance.puntos = 0;
        GameController.Instance.vidas = 3;
        PlayerPrefs.SetInt("PlayerScore", 0);
        PlayerPrefs.SetInt("PlayerLives", 3);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}
