using UnityEngine;

public class FixedEnemy : MonoBehaviour
{
    public int health = 10; // Enemy's health
    public int damageToPlayer = 2; // Damage to the player

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el enemigo colisiona con el jugador, hacer daño al jugador
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.TakeDamage(damageToPlayer);
            }
        }

        // Si el enemigo colisiona con una bala, recibir daño
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage); // Recibir daño de la bala
                other.gameObject.SetActive(false); // Desactivar la bala
            }
        }
    }

    // Método para aplicar daño al enemigo
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DestroyEnemy(); // Destruir al enemigo si su salud llega a 0
        }
    }

    // Destruir al enemigo
    private void DestroyEnemy()
    {
        Destroy(gameObject); // Eliminar el enemigo del juego
    }
}
