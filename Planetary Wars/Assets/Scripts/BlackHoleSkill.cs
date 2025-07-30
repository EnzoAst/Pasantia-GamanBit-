using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkill : MonoBehaviour
{
    public float lifeSpan = 5f;
    public float suctionForce = 5f;
    public float suctionRadius = 3f;
    public int damagePerSecond = 10;
    public float shrinkSpeed = 0.5f;

    private float blackHoleLifeSpan;
    private Animator blackHoleAnimator;
    private List<Transform> enemiesToShrink = new List<Transform>();

    #region Animation Hash ID's
    private readonly int blackHoleLifeSpanID = Animator.StringToHash("LifeSpan");
    #endregion

    public void SetLifeSpan(float newLifeSpan)
    {
        lifeSpan = newLifeSpan;
        Debug.Log($"BlackHole lifespan actualizado a: {lifeSpan}");
    }

    public void SetSuctionForce(float newSuctionForce)
    {
        suctionForce = newSuctionForce;
        Debug.Log($"BlackHole suction force actualizada a: {suctionForce}");
    }

    private void Start()
    {
        // Aplicar upgrades acumulados
        SetLifeSpan(lifeSpan + UpgradeDataManager.Instance.GetBlackHoleUpgrade("lifespan"));
        SetSuctionForce(suctionForce + UpgradeDataManager.Instance.GetBlackHoleUpgrade("suction"));

        blackHoleLifeSpan = lifeSpan;
        blackHoleAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (SkillPanelController.IsSkillPanelOpen)
            return;

        if (blackHoleLifeSpan > 0f)
        {
            if (blackHoleLifeSpan == lifeSpan)
                blackHoleAnimator.Play("BlackHole");

            blackHoleLifeSpan -= Time.deltaTime;
            blackHoleAnimator.SetFloat(blackHoleLifeSpanID, blackHoleLifeSpan);

            // Succionar enemigos mientras el agujero negro esté activo
            SuctionEnemies();
        }
        else
        {
            Destroy(); // Destruir cuando el tiempo termina
        }
    }

    private void SuctionEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, suctionRadius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    // Calcular dirección y fuerza de succión
                    Vector2 direction = (transform.position - enemy.transform.position).normalized;
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);
                    float force = suctionForce / Mathf.Max(distance, 0.1f);
                    enemyRb.AddForce(direction * force, ForceMode2D.Force);
                }

                // Achicar al enemigo
                EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
                if (enemyBehavior != null)
                {
                    enemyBehavior.Shrink(shrinkSpeed);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                // Daño gradual por segundo
                enemy.TakeDamage(Mathf.FloorToInt(damagePerSecond * Time.deltaTime));
            }
        }
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    // Para visualizar el radio de succión en la escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, suctionRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.SetSucked(true); // ¡Detener el movimiento de caminar!
                enemy.Shrink(shrinkSpeed); // Achicar al enemigo
                Destroy(enemy.gameObject); // Destruir el enemigo al colisionar
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.SetSucked(false); // Reanudar el movimiento al salir del agujero negro
            }
        }
    }
}