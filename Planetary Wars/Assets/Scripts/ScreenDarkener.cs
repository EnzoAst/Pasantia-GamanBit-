using UnityEngine;

public class ScreenDarkener : MonoBehaviour
{
    public static ScreenDarkener instance;

    private CanvasGroup canvasGroup; // Para manejar la transparencia de la imagen

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogError("No se encontró un CanvasGroup en el objeto ScreenDarkener. Asegúrate de agregarlo.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeIn(float duration)
    {
        gameObject.SetActive(true); // Activar la imagen de oscurecimiento
        StartCoroutine(Fade(0, 1, duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(Fade(1, 0, duration));
    }

    private System.Collections.IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;

        if (to == 0)
        {
            gameObject.SetActive(false); // Desactivar la imagen si se desvanece completamente
        }
    }
}