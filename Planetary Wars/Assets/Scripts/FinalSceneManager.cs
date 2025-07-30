using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalSceneManager : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainScreen");
    }
}