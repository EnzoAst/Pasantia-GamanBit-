using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DestroyPlayer : MonoBehaviour
{
    public Transform sun;  // Referencia al objeto Sol
    public float destructionDistance = 1.5f;  // Distancia para la destrucción
    public AudioClip touchSunSound;  // Sonido al tocar el Sol
    public int livesToLose = 3;  // Vidas a perder al tocar el Sol
    public GameObject explosionEffect;  // Efecto de explosión
    public float blinkDuration = 0.2f; // Duración de cada parpadeo
    public int blinkCount = 2;  // Cantidad de parpadeos antes de morir

    private bool hasTouchedSun = false;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Verifica la distancia al sol y si no ha tocado el Sol
        if (Vector2.Distance(transform.position, sun.position) < destructionDistance && !hasTouchedSun)
        {
            // Marca que ha tocado el Sol
            hasTouchedSun = true;

            // Inicia la corrutina para manejar el evento de tocar el Sol
            StartCoroutine(OnTouchSun());
        }
    }

    IEnumerator OnTouchSun()
    {
        // Desactiva el control del jugador y congela su movimiento
        GetComponent<PlayerController>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;  // Congela el Rigidbody para evitar cualquier movimiento

        // Instanciar el efecto de explosión de inmediato
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Reproducir el sonido al tocar el Sol
        if (touchSunSound != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();
            tempAudioSource.clip = touchSunSound;
            tempAudioSource.Play();
            DontDestroyOnLoad(tempAudio);
            Destroy(tempAudio, touchSunSound.length);
        }

        // Reducir las vidas
        ReducirVidas();

        // Espera breve antes de parpadear
        yield return new WaitForSeconds(0.1f);

        // Parpadeo
        for (int i = 0; i < blinkCount * 2; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkDuration);
        }

        // Asegúrate de que el jugador esté visible antes de continuar
        spriteRenderer.enabled = true;

        // Esperar la duración del sonido antes de continuar (si es necesario)
        if (touchSunSound != null)
        {
            yield return new WaitForSeconds(touchSunSound.length - (blinkCount * blinkDuration * 2));
        }
        else
        {
            yield return new WaitForSeconds(1f);  // Espera predeterminada si no hay sonido
        }

        // Destruir el jugador después de que el sonido haya terminado
        Destroy(gameObject);

        // Cargar la escena de Game Over si el jugador ha perdido todas las vidas
        if (GameController.Instance.vidas <= 0)
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }

    void ReducirVidas()
    {
        for (int i = 0; i < livesToLose; i++)
        {
            GameController.Instance.RestarVida();
        }
    }
}
