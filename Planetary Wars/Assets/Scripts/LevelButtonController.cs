using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelButtonController : MonoBehaviour
{
    public void LoadLevelByName(string sceneName)
    {
        StartCoroutine(LoadLevelWithDelay(sceneName));
    }

    private IEnumerator LoadLevelWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}