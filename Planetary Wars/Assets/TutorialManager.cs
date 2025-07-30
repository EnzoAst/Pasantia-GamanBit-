using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject storyPanel;

    private bool tutorialActive = false;

    void Awake()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(WaitForStoryPanelToCloseThenShowTutorial());
    }

    IEnumerator WaitForStoryPanelToCloseThenShowTutorial()
    {
        // Espera a que el storyPanel se active (por si aparece tarde)
        while (storyPanel != null && !storyPanel.activeSelf)
        {
            yield return null;
        }

        // Espera a que el storyPanel se cierre
        while (storyPanel != null && storyPanel.activeSelf)
        {
            yield return null;
        }

        // Muestra el tutorialPanel y pausa el juego
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
            tutorialActive = true;
        }
    }

    public void CloseTutorial()
    {
        if (!tutorialActive) return;

        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        tutorialActive = false;
    }
}
