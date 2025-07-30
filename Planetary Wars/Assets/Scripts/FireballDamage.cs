using UnityEngine;

public class FireballDamage : MonoBehaviour
{
    public int damage = 20; // Daño que inflige la bola de fuego
    public GameObject effectOnTarget; // Prefab del efecto visual en el objetivo
    public float effectDuration = 2f; // Duración del efecto visual

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si colisiona con el jugador o un aliado
        if (collision.CompareTag("Player") || collision.CompareTag("Ally"))
        {
            // Aplicar daño a través del GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("GameManager no encontrado. No se puede aplicar daño.");
            }

            // Instanciar efecto visual en el objetivo
            if (effectOnTarget != null)
            {
                GameObject effect = Instantiate(effectOnTarget, collision.transform.position, Quaternion.identity);
                effect.transform.SetParent(collision.transform); // Hacer que el efecto siga al objetivo
                Destroy(effect, effectDuration); // Destruir efecto tras la duración
            }
        }
    }
}