using UnityEngine;
using UnityEngine.SceneManagement;

public class GravitySettingsManager : MonoBehaviour
{
    void Awake()
    {
        SceneGravity.SetGravityForScene(SceneManager.GetActiveScene().name);
    }
}
