using UnityEngine;
using TMPro;

public class FlotationText : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float fadeDuration = 1f;

    private TMP_Text textComponent;
    private float startTime;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();  // Se obtiene el componente TMP_Text
        if (textComponent == null)
        {
            Debug.LogError("No TMP_Text component found on the FlotationText prefab.");
            Destroy(gameObject);  // Destruye el objeto si no tiene el componente TMP_Text
            return;
        }

        startTime = Time.time;
    }

    void Update()
    {
        float elapsed = Time.time - startTime;

        // Movimiento hacia arriba
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade-out
        float alpha = 1 - (elapsed / fadeDuration);
        Color newColor = textComponent.color;
        newColor.a = Mathf.Clamp01(alpha);
        textComponent.color = newColor;

        // Destruir después de que se desvanezca
        if (elapsed >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}