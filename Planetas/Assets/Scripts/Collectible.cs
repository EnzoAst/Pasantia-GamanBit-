using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocidad de rotaci�n
    private bool fueRecogido = false; // Marca si la estrella ha sido recogida

    private AudioSource audioSource; // Variable para el AudioSource

    void Start()
    {
        // Intenta obtener el AudioSource del GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en el GameObject: " + gameObject.name);
        }
    }

    void Update()
    {
        // Rota la estrella continuamente en el eje Z
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Colisi�n con: " + other.gameObject.name);

        // Verifica si el objeto que colisiona es el jugador y si no ha sido recogido a�n
        if (other.CompareTag("Player") && !fueRecogido)
        {
            // Sumar puntos al jugador
            GameController.Instance.SumarPuntos(100);
            Debug.Log("Collectible recogido. Puntos: " + GameController.Instance.puntos);

            // A�adir la posici�n de la estrella al GameManager
            GameManager.Instance.AddEstrella(transform.position);

            // Reproducir el sonido de recogida
            audioSource.Play();

            // Destruir la estrella despu�s de que suene el audio
            Destroy(gameObject, audioSource.clip.length); // Destruir despu�s de que termine el sonido

            // Marca el collectible como recogido
            fueRecogido = true;
        }
    }
}
