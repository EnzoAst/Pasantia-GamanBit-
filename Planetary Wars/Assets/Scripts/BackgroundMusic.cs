using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    private static bool musicExists = false;

    void Awake()
    {
        if (musicExists)
        {
            Destroy(gameObject);
            return;
        }

        musicExists = true;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nombres de las 3 primeras escenas donde querés que suene la música
        if (scene.name != "MainScreen" && scene.name != "MainMenu" && scene.name != "History")
        {
            Destroy(gameObject);
            musicExists = false;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
