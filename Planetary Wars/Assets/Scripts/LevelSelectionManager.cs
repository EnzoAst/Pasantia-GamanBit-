using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Botones de Niveles")]
    public Button[] levelButtons;

    void Start()
    {
        int lastLevelCompleted = PlayerPrefs.GetInt("LastLevelCompleted", 0);

        bool unlock = lastLevelCompleted == 1;

        foreach (Button btn in levelButtons)
        {
            btn.interactable = unlock;
        }
    }
}