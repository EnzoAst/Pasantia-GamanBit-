using UnityEngine;
using System.Collections;

public class ThunderDamage : MonoBehaviour
{
    public int damage = 20;
    public GameObject effectOnPlayer;
    public float effectDuration = 2f;
    public GameObject burnedPrefab;
    public float burnDuration = 4f;
    public float offsetY = 0.5f; // Cuánto baja la quemadura

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Ally"))
        {
            GameManager.instance.TakeDamage(damage);

            if (effectOnPlayer != null)
            {
                GameObject effect = Instantiate(effectOnPlayer, collision.transform.position, Quaternion.identity);
                effect.transform.SetParent(collision.transform);
                Destroy(effect, effectDuration);
            }
        }

        StartCoroutine(DestroyAfterDelay(2f)); // Ahora espera 2 segundos
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
            return; // No hacer nada si la escena se está cerrando

        // Verificamos si la escena se está cerrando
        if (gameObject.scene.isLoaded == false)
            return;

        if (burnedPrefab != null)
        {
            Vector3 burnPosition = new Vector3(transform.position.x, transform.position.y - offsetY, transform.position.z);
            GameObject burn = Instantiate(burnedPrefab, burnPosition, Quaternion.identity);
            Destroy(burn, burnDuration);
        }
    }


}
