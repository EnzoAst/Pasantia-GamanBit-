using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillHealthBar; // Cambiado a Image porque Image es correcto para UI
    private float maximumLife;

    private void Start()
    {
        // Desactiva este script si est� en el men� principal
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            this.enabled = false;
            return;
        }

        // Obtenemos la vida m�xima del jugador desde el GameManager
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager no encontrado. Desactivando HealthBar.");
            this.enabled = false;
            return;
        }

        maximumLife = GameManager.instance.playerLife;

        if (fillHealthBar == null)
        {
            Debug.LogWarning("fillHealthBar no est� asignado en el Inspector. Desactivando HealthBar.");
            this.enabled = false;
        }
    }

    private void Update()
    {
        // Verifica si la escena no es el men� principal
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        // Aseg�rate de que GameManager y fillHealthBar no sean null antes de actualizar
        if (GameManager.instance == null || fillHealthBar == null)
            return;

        // Actualizamos el fillAmount de la barra de vida con base en la vida actual del jugador
        fillHealthBar.fillAmount = (float)GameManager.instance.playerLife / maximumLife;
    }
}