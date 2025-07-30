using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Panel de Ayuda")]
    public GameObject panelHelp;
    public Button buttonHelp;
    public Button buttonCloseHelp;

    private void Start()
    {
        // Asignar listeners a los botones si están asignados en el inspector
        if (buttonHelp != null)
            buttonHelp.onClick.AddListener(ShowHelpPanel);

        if (buttonCloseHelp != null)
            buttonCloseHelp.onClick.AddListener(CloseHelpPanel);

        if (panelHelp != null)
            panelHelp.SetActive(false); // Ocultar el panel al inicio
    }

    public void ShowHelpPanel()
    {
        if (panelHelp != null)
            panelHelp.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        if (panelHelp != null)
            panelHelp.SetActive(false);
    }

    public void Back()
    {
        Invoke("LoadMainScreen", 1f);
    }

    private void LoadMainScreen()
    {
        SceneManager.LoadScene("MainScreen");
    }

    public void Play()
    {
        Invoke("LoadNextScene", 1f);
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");

        PlayerPrefs.DeleteKey("LastLevelCompleted");
        PlayerPrefs.Save();
        Debug.Log("Niveles bloqueados nuevamente.");

        Invoke("DoQuit", 1f);
    }

    private void DoQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}