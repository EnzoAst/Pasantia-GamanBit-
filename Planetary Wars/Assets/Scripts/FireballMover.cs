using UnityEngine;

public class FireballMover : MonoBehaviour
{
    private float speed;
    private float endY; // Punto final aleatorio en el eje Y
    public GameObject explosionPrefab; // Prefab de explosión
    public GameObject burned; // Prefab quemadura
    public GameObject burnedCarrier; // Prefab de quemadura especial para objetos "Carrier"
    public float detectionRadius = 0.5f; // Radio para detectar objetos con etiqueta "fire"

    // Método para velocidad
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // Método para configurar el punto final en Y
    public void SetEndY(float newEndY)
    {
        endY = newEndY;
    }

    private void Update()
    {
        // Mover la bola de fuego hacia abajo en línea recta
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Destruir la bola de fuego si alcanza su punto final en Y
        if (transform.position.y <= endY)
        {
            TriggerExplosion();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detectar colisión con el Player
        if (other.CompareTag("Player"))
        {
            TriggerExplosion();
            Destroy(gameObject);
        }
    }

    private void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Fire"))
                {
                    return; // No dejar ninguna quemadura
                }
                else if (hit.CompareTag("Carrier"))
                {
                    // Dejar quemadura especial en carrier
                    if (burnedCarrier != null)
                    {
                        Vector3 burnedPosition = new Vector3(transform.position.x, endY, transform.position.z);
                        GameObject burnedInstance = Instantiate(burnedCarrier, burnedPosition, Quaternion.identity);
                        Destroy(burnedInstance, 4f);
                        return;
                    }
                }
            }

            // Si no es Fire ni Carrier, dejar quemadura normal
            if (burned != null)
            {
                Vector3 burnedPosition = new Vector3(transform.position.x, endY, transform.position.z);
                GameObject burnedInstance = Instantiate(burned, burnedPosition, Quaternion.identity);
                Destroy(burnedInstance, 4f);
            }
        }
    }


    private void OnDrawGizmos()
    {
        // Dibujar el radio de detección en el editor para depuración
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}