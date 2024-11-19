using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonSound()
    {
        audioSource.Play(); // Reproduce el sonido
    }

    public void StartGame()
    {
        StartCoroutine(PlaySoundAndStartGame()); // Inicia la coroutine al pulsar el botón
    }

    private IEnumerator PlaySoundAndStartGame()
    {
        PlayButtonSound(); // Reproducir el sonido
        yield return new WaitForSeconds(audioSource.clip.length); // Esperar a que termine el sonido
        SceneManager.LoadScene("Level1"); // Cambia a tu escena de juego
    }

    public void QuitGame()
    {
        StartCoroutine(PlaySoundAndQuitGame()); // Inicia la coroutine para salir del juego
    }

    private IEnumerator PlaySoundAndQuitGame()
    {
        PlayButtonSound(); // Reproducir el sonido
        yield return new WaitForSeconds(audioSource.clip.length); // Esperar a que termine el sonido
        Debug.Log("Saliendo del juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Detiene la ejecución del juego en el editor, simulando que sale del juego.
#else
        Application.Quit(); // Cierra la aplicación en compilaciones cuando exportemos el juego.
#endif
    }
}
