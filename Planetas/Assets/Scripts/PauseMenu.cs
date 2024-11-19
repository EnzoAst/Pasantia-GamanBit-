using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Asigna aquí tu panel de pausa desde el Inspector

    private bool isPaused = false;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonSound()
    {
        audioSource.Play(); // Reproduce el sonido
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Cambia el estado de pausa al presionar Escape
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Oculta el menú de pausa
        Time.timeScale = 1f; // Reanuda el tiempo del juego
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true); // Muestra el menú de pausa
        Time.timeScale = 0f; // Detiene el tiempo del juego
        isPaused = true;
    }

    public void QuitGame()
    {
        StartCoroutine(PlaySoundAndQuitGame()); // Inicia la coroutine para salir del juego
    }

    private IEnumerator PlaySoundAndQuitGame()
    {
        PlayButtonSound(); // Reproducir el sonido
        yield return new WaitForSecondsRealtime(audioSource.clip.length); // Esperar a que termine el sonido
        Debug.Log("Saliendo del juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Detiene la ejecución del juego en el editor, simulando que sale del juego.
#else
        Application.Quit(); // Cierra la aplicación en compilaciones cuando exportamos el juego.
#endif
    }
}
