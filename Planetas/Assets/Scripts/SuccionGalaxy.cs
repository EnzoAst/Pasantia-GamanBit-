using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuccionGalaxy : MonoBehaviour
{
    public Transform galaxyOPortal;           // Referencia al objeto Galaxy
    public float succionDistance = 1.5f;      // Distancia para la succión
    public GameObject explosionEffect;        // Prefab del efecto de explosión
    public string nextLevel = "Nivel2";       // Nombre de la escena del siguiente nivel
    public AudioClip cambioGalaxiaSound;      // Clip de audio para el cambio de galaxia

    private AudioSource audioSource;          // Referencia al AudioSource

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  // Obtener el AudioSource del mismo GameObject
    }

    void Update()
    {
        // Verifica la distancia a la galaxia
        if (Vector2.Distance(transform.position, galaxyOPortal.position) < succionDistance)
        {
            // Reproduce el sonido de cambio de galaxia si está asignado
            if (cambioGalaxiaSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(cambioGalaxiaSound);
            }

            // Instancia el efecto de explosión
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Simula que el personaje es succionado y transfiere al siguiente nivel
            StartCoroutine(LoadNextSceneWithDelay(1f)); // Cambia a la escena del siguiente nivel después de 1 segundo
        }
    }

    private IEnumerator LoadNextSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Esperar antes de cambiar de escena
        SceneManager.LoadScene(nextLevel); // Carga la escena del siguiente nivel
    }
}
