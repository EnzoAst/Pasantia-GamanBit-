using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 10f; // Velocidad del misil
    public float rotationSpeed = 200f; // Velocidad de rotaci�n para seguir al objetivo
    public float lifetime = 5f; // Tiempo de vida del misil
    public GameObject explosionPrefab; // Prefab de la explosi�n
    public int damage = 100; // Da�o que hace el misil

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifetime); // Destruir el misil despu�s de un tiempo
    }

    void Update()
    {
        if (target == null)
        {
            // Si no encuentra un objetivo, simplemente avanza recto
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            return;
        }

        // Direcci�n hacia el objetivo
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        direction.Normalize();

        // Rotar hacia el objetivo
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        transform.Rotate(0, 0, -rotateAmount * rotationSpeed * Time.deltaTime);

        // Mover el misil hacia adelante
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Si colisiona con un enemigo o un jefe
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            // Instanciar la explosi�n al impactar
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 2f); // Destruir la explosi�n despu�s de 2 segundos
            }

            // Intentar aplicar da�o al objetivo
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Reducir la vida del enemigo
            }

            // Destruir el misil despu�s del impacto
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}