using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalLevels : MonoBehaviour
{
    public string escenaDestino = "Congratulations"; // Nombre de la escena destino
    public AudioClip sonidoTeleportacion;      // Sonido de teleportación al tocar el portal
    public float delayAntesDeCambiarEscena = 1.5f; // Tiempo de espera antes de cambiar de escena

    private AudioSource audioSource;            // Referencia al AudioSource
    private Rigidbody2D playerRigidbody;        // Referencia al Rigidbody2D del jugador

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Obtener el AudioSource del mismo GameObject
        if (audioSource == null)
        {
            // Agregar AudioSource si no está presente
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verificar si el jugador ha tocado el Portal
        {
            // Obtener el Rigidbody2D del jugador y desactivar su movimiento
            playerRigidbody = other.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector2.zero; // Detener el movimiento
                playerRigidbody.isKinematic = true;      // Desactivar la física del jugador
            }

            // Reproducir el sonido de teleportación si está asignado y el AudioSource está configurado
            if (sonidoTeleportacion != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoTeleportacion);
            }

            // Esperar antes de cambiar de escena para que el sonido se reproduzca completo
            Invoke("TeleportarJugadorAEscena", delayAntesDeCambiarEscena);
        }
    }

    void TeleportarJugadorAEscena()
    {
        SceneManager.LoadScene(escenaDestino); // Cargar la escena destino
    }
}
