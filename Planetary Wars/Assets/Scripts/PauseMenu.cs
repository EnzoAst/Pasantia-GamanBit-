using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Panel de pausa
    private bool isPaused = false;
    private AudioSource audioSource; // Audio del menú de pausa
    private AudioSource[] allAudioSources; // Todos los audios en la escena
    private List<AudioSource> loopingAudioSources = new List<AudioSource>(); // Lista de audios en bucle
    public static bool escapeEnabled = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!escapeEnabled) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Verificamos si hay panel de victoria o derrota activo en GameManager
            if (GameManager.instance != null &&
                (GameManager.instance.IsVictoryPanelActive() || GameManager.instance.IsDefeatPanelActive()))
            {
                return; // No hacer nada si hay panel de victoria o derrota activo
            }

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

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        PauseAllAudioSources();
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        ResumeAllAudioSources();
        isPaused = false;
    }

    public void QuitGame()
    {
        StartCoroutine(PlaySoundAndLoadMainMenu());
    }

    private IEnumerator PlaySoundAndLoadMainMenu()
    {
        PlayButtonSound(); // Reproducir el sonido del botón
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        Debug.Log("Cargando MainMenu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayButtonSound()
    {
        audioSource.Play();
    }

    private void PauseAllAudioSources()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();
        loopingAudioSources.Clear();

        foreach (AudioSource source in allAudioSources)
        {
            // Ignora la música de fondo
            if (source.CompareTag("Music"))
                continue;

            if (source.isPlaying)
            {
                if (source.loop)
                {
                    loopingAudioSources.Add(source);
                }
                source.Pause();
            }
        }
    }

    private void ResumeAllAudioSources()
    {
        if (allAudioSources != null)
        {
            foreach (AudioSource source in allAudioSources)
            {
                if (source == null) continue;
                if (source.CompareTag("Music")) continue; // Ignora la música de fondo

                if (loopingAudioSources.Contains(source))
                {
                    source.Play();
                }
                else
                {
                    source.UnPause();
                }
            }
        }
    }

    public void TogglePauseFromButton()
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