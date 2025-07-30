using UnityEngine;

public class Lava : MonoBehaviour
{
    public int damageToPlayer;
    public GameObject firePrefab;

    private GameObject activeFireEffect;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                if (!player.isJumping) // Solo si no está saltando
                {
                    GameManager.instance.TakeDamage(damageToPlayer);
                    HandleFireEffect(other.transform);
                }
            }
        }
        else if (other.CompareTag("Ally"))
        {
            // Verificamos si es un Ally
            AllyController ally = other.GetComponent<AllyController>();
            if (ally != null)
            {
                ally.TakeDamage(damageToPlayer); // Resta vida al Ally
                HandleFireEffect(other.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Ally")) && activeFireEffect != null)
        {
            Destroy(activeFireEffect);
            activeFireEffect = null;
        }
    }

    private void HandleFireEffect(Transform target)
    {
        if (activeFireEffect == null)
        {
            Vector3 firePosition = target.position + new Vector3(0f, -0.35f, 0);
            activeFireEffect = Instantiate(firePrefab, firePosition, Quaternion.identity, target);
        }
    }
}