using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryPanelsController : MonoBehaviour
{
    // Referencias a los paneles
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;

    // Método para mostrar el panel 2
    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }

    // Método para mostrar el panel 3
    public void ShowPanel3()
    {
        panel2.SetActive(false);
        panel3.SetActive(true);
    }

    // Método para mostrar el panel 3
    public void ShowPanel4()
    {
        panel3.SetActive(false);
        panel4.SetActive(true);
    }

    // Método para cargar la siguiente escena
    public void LoadNextScene()
    {
        SceneManager.LoadScene("Mars"); // Cambia "NextSceneName" por el nombre de tu escena
    }
}