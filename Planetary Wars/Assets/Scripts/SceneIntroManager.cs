using UnityEngine;

public class SceneIntroManager : MonoBehaviour
{
    [SerializeField] private GameObject storyPanel;

    void Start()
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(true);
            PauseMenu.escapeEnabled = false;
        }
        else
        {
            Debug.LogWarning("Story Panel no asignado en SceneIntroManager.");
        }

        AudioSource music = GameObject.FindWithTag("Music").GetComponent<AudioSource>();
        if (music != null) music.ignoreListenerPause = true;

        // Pausa el juego
        Time.timeScale = 0f;
        Debug.Log("Juego pausado para intro.");
    }

    public void ContinueGame()
    {
        // Desactiva el panel de historia
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
            PauseMenu.escapeEnabled = true;
        }

        // Reanuda el juego
        Time.timeScale = 1f;
        Debug.Log("Juego reanudado después de intro.");
    }
}