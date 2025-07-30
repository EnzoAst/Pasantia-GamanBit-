using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public int damageAmount = 10; // Da�o que hace el fuego
    public float damageInterval = 0.5f; // Tiempo entre cada da�o (si el jugador sigue en contacto)
    private float nextDamageTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            GameManager.instance.TakeDamage(damageAmount);
            nextDamageTime = Time.time + damageInterval; // Espera antes de volver a hacer da�o
        }
    }
}