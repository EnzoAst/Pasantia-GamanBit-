using UnityEngine;
using System.Collections;

public class IceRiver : MonoBehaviour
{
    public int damageToEntity = 5;
    public GameObject iceCubePrefab;
    public float freezeDuration = 3f;

    [Header("Audio")]
    public AudioClip freezeSound;
    public AudioClip unfreezeSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                if (!player.isJumping) // Solo si no está saltando
                {
                    StartCoroutine(FreezeEntity(player.gameObject, player.transform));
                    GameManager.instance.TakeDamage(damageToEntity);
                }
            }
        }
        else if (other.CompareTag("Ally"))
        {
            AllyController ally = other.GetComponent<AllyController>();
            if (ally != null)
            {
                {
                    StartCoroutine(FreezeEntity(ally.gameObject, ally.transform));
                    ally.TakeDamage(damageToEntity);
                }
            }
        }
    }

    private IEnumerator FreezeEntity(GameObject entity, Transform targetTransform)
    {
        // Reproducir sonido de congelar
        if (freezeSound != null)
        {
            AudioSource.PlayClipAtPoint(freezeSound, targetTransform.position);
        }

        // Pausar movimiento
        MonoBehaviour controller = entity.GetComponent<MonoBehaviour>();
        if (controller != null) controller.enabled = false;

        // Pausar animación
        Animator animator = entity.GetComponent<Animator>();
        if (animator != null) animator.enabled = false;

        // Crear cubo de hielo
        Vector3 icePosition = targetTransform.position;
        GameObject iceEffect = Instantiate(iceCubePrefab, icePosition, Quaternion.identity, targetTransform);

        // Esperar congelamiento
        yield return new WaitForSeconds(freezeDuration);

        // Quitar cubo de hielo
        Destroy(iceEffect);

        // Reactivar movimiento y animación
        if (controller != null) controller.enabled = true;
        if (animator != null) animator.enabled = true;

        // Reproducir sonido de descongelar
        if (unfreezeSound != null)
        {
            AudioSource.PlayClipAtPoint(unfreezeSound, targetTransform.position);
        }
    }
}