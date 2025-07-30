using UnityEngine;

public class WaveAbility : MonoBehaviour
{
    public float waveRadius = 5f; // Radio de la onda expansiva
    public float waveForce = 10f; // Fuerza de la onda expansiva
    public LayerMask enemyLayer; // Capa de enemigos
    private CircleCollider2D myCollider;

    void Start()
    {
        // Aplicar upgrades acumulados
        SetWaveForce(waveForce + UpgradeDataManager.Instance.GetWaveUpgrade("force"));
        SetWaveRadius(waveRadius + UpgradeDataManager.Instance.GetWaveUpgrade("radius"));

        // Configurar el collider
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = waveRadius;
    }

    public void SetWaveForce(float newForce)
    {
        waveForce = newForce;
    }

    public void SetWaveRadius(float newRadius)
    {
        waveRadius = newRadius;
    }

    // Método para activar la onda expansiva
    public void ActivateWave()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, waveRadius, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 direction = enemy.transform.position - transform.position;
                enemyRb.AddForce(direction.normalized * waveForce, ForceMode2D.Impulse);
            }

            EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.RecoverAfterPush(3f); // se recupera a los 3 segundos
            }
        }

        // Destruir el objeto luego de un tiempo
        Destroy(gameObject, 0.5f);
    }

    // Método para dibujar el radio de la onda expansiva en la vista de escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, waveRadius);
    }
}