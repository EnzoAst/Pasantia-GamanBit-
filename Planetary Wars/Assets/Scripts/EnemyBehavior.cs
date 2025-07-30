using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    private Transform player;
    private Animator animator;
    public int health = 100;

    public float speed = 3f;
    public float attackRadius = 1.5f;

    private bool isAttacking = false;
    private bool facingRight = true;
    private bool isDead = false;

    public LayerMask deadEnemyLayer;

    private Coroutine attackCoroutine;
    private bool isSucked = false;

    private bool canAttack = true;
    public GameObject dropPrefab;
    public int attackDamage;
    public AudioClip playerHurtSound;
    private AudioSource audioSource;

    // -------- Barras de vida SOLO para Boss --------
    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private Image healthFill;
    private CanvasGroup healthBarCanvasGroup;
    private Coroutine hideBarCoroutine;
    private int maxHealth;
    public Vector3 healthBarOffset = new Vector3(0, -1.5f, 0);
    public AudioClip attackSound;
    public AudioClip deathSound;

    void Start()
    {
        //IgnoreCollisionsWithOtherEnemies();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }

        maxHealth = health;

        // Si no se inicializó el player, intentar buscarlo automáticamente
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }

        // Solo instanciar barra si este es un Boss
        if (CompareTag("Boss") && healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, null);
            healthBarInstance.transform.localScale = Vector3.one;
            healthBarInstance.transform.rotation = Quaternion.identity;

            healthFill = healthBarInstance.transform.Find("Background/Fill").GetComponent<Image>();
            if (healthFill != null)
                healthFill.fillAmount = 1f;

            healthBarCanvasGroup = healthBarInstance.GetComponent<CanvasGroup>();
            if (healthBarCanvasGroup == null)
                healthBarCanvasGroup = healthBarInstance.AddComponent<CanvasGroup>();

            healthBarCanvasGroup.alpha = 0f;
            healthBarInstance.SetActive(true);
        }
    }

    public void Initialize(Transform playerTarget)
    {
        player = playerTarget;
    }

    void Update()
    {
        if (isDead || isSucked) return;

        Transform target = GetClosestTarget();
        if (target == null) return;

        // Si es Ally y está muerto, ignorar
        if (target.CompareTag("Ally"))
        {
            AllyController ally = target.GetComponent<AllyController>();
            if (ally != null && ally.isDead)
                return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRadius)
        {
            if (!isAttacking)
            {
                attackCoroutine = StartCoroutine(AttackPlayer(target));
            }

            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
        }
        else
        {
            if (isAttacking)
            {
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                }
                isAttacking = false;

                if (animator != null)
                {
                    animator.SetBool("IsWalking", true);
                }
            }

            Vector2 direction = (target.position - transform.position).normalized;

            // Parámetros
            float checkRadius = 5f;
            float castDistance = 2.0f;
            LayerMask obstacleMask = LayerMask.GetMask("Environment");

            // Obstrucción al frente
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, checkRadius, direction, castDistance, obstacleMask);

            if (hit.collider != null)
            {
                Vector2[] directionsToTry = new Vector2[]
                {
        Vector2.left,
        Vector2.right,
        Vector2.up,
        Vector2.down,
        new Vector2(-1, 1).normalized,  // ↖
        new Vector2(1, 1).normalized,   // ↗
        new Vector2(-1, -1).normalized, // ↙
        new Vector2(1, -1).normalized   // ↘
                };

                Vector2 bestAlternative = direction;
                float bestDot = -1f;

                foreach (var dir in directionsToTry)
                {
                    RaycastHit2D altHit = Physics2D.CircleCast(transform.position, checkRadius, dir, 0.6f, obstacleMask);
                    if (altHit.collider == null)
                    {
                        float dot = Vector2.Dot((direction + dir).normalized, direction);
                        if (dot > bestDot)
                        {
                            bestDot = dot;
                            bestAlternative = dir;
                        }
                    }
                }

                direction = (direction + bestAlternative).normalized;
            }

            // Mover
            transform.position += (Vector3)(direction * speed * Time.deltaTime);




            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }

            if (direction.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    void LateUpdate()
    {
        if (CompareTag("Boss") && healthBarInstance != null)
        {
            Vector3 offset = healthBarOffset;
            healthBarInstance.transform.position = transform.position + offset;
            healthBarInstance.transform.localScale = Vector3.one;
            healthBarInstance.transform.rotation = Quaternion.identity;
        }
    }

    public void SetSucked(bool state)
    {
        isSucked = state;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ally"))
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();
            Collider2D otherCollider = collision.collider;

            if (enemyCollider != null && otherCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, otherCollider);
            }
        }
    }

    IEnumerator AttackPlayer(Transform target)
    {
        isAttacking = true;
        canAttack = true;

        AllyController ally = null;
        if (target.CompareTag("Ally"))
            ally = target.GetComponent<AllyController>();

        while (target != null && Vector2.Distance(transform.position, target.position) <= attackRadius)
        {
            if (ally != null && ally.isDead)
                break;

            if (canAttack)
            {
                if (animator != null)
                    animator.SetTrigger("Attack");
                canAttack = false; // Bloquea hasta que termine la animación
            }
            yield return null;
        }

        isAttacking = false;
        if (animator != null)
            animator.SetBool("IsWalking", true);
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void OnAttackAnimationEnd()
    {
        canAttack = true;
    }

    // Este método se llama desde el AnimationEvent
    public void DealDamageToPlayer()
    {
        Transform target = GetClosestTarget();
        DealDamageToPlayer(target);
    }

    public void DealDamageToPlayer(Transform target)
    {
        if (isDead || target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= attackRadius)
        {
            if (target.CompareTag("Player"))
            {
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.TakeDamage(attackDamage);
                }
                if (playerHurtSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(playerHurtSound);
                }
            }
            else if (target.CompareTag("Ally"))
            {
                AllyController ally = target.GetComponent<AllyController>();
                if (ally != null && !ally.isDead) // Solo dañar si el Ally está vivo
                {
                    ally.TakeDamage(attackDamage);
                }
            }
        }
    }

    private Transform GetClosestTarget()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        // Player
        if (player != null)
        {
            float playerDist = Vector2.Distance(transform.position, player.position);
            if (playerDist < minDistance)
            {
                minDistance = playerDist;
                closest = player;
            }
        }

        // Allies
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject allyObj in allies)
        {
            if (allyObj == null) continue;
            AllyController allyController = allyObj.GetComponent<AllyController>();
            if (allyController != null && allyController.isDead) continue;
            float dist = Vector2.Distance(transform.position, allyObj.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = allyObj.transform;
            }
        }
        return closest;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        // Sólo mostrar o actualizar barra si es Boss
        if (CompareTag("Boss") && healthBarInstance != null)
        {
            ShowHealthBarFade();

            if (healthFill != null)
                healthFill.fillAmount = Mathf.Clamp01((float)health / maxHealth);

            if (hideBarCoroutine != null)
                StopCoroutine(hideBarCoroutine);

            hideBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(2f, 0.7f));
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator HideHealthBarAfterDelay(float delay, float fadeDuration)
    {
        yield return new WaitForSeconds(delay);
        yield return FadeHealthBar(healthBarCanvasGroup.alpha, 0f, fadeDuration);
    }

    private void ShowHealthBarFade()
    {
        if (hideBarCoroutine != null)
        {
            StopCoroutine(hideBarCoroutine);
            hideBarCoroutine = null;
        }
        StartCoroutine(FadeHealthBar(healthBarCanvasGroup.alpha, 1f, 0.2f));
    }

    private IEnumerator FadeHealthBar(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (healthBarCanvasGroup == null)
                yield break;
            healthBarCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        if (healthBarCanvasGroup != null)
            healthBarCanvasGroup.alpha = to;
    }

    public void Shrink(float shrinkSpeed)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);
        if (transform.localScale.magnitude < 0.05f)
        {
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        isDead = true;
        speed = 0f;
        isAttacking = false;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        DropItem();
        IgnoreCollisionWithPlayer();

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }

        StartCoroutine(WaitForDeathAnimation());
        GameManager.instance.CheckVictoryConditionAfterEnemyDeath();
    }

    private void DropItem()
    {
        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
    }

    private void IgnoreCollisionWithPlayer()
    {
        Collider2D playerCollider = player != null ? player.GetComponent<Collider2D>() : null;
        Collider2D enemyCollider = GetComponent<Collider2D>();

        if (playerCollider != null && enemyCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, enemyCollider);
        }
    }

    /*private void IgnoreCollisionsWithOtherEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject)
            {
                Collider2D thisCollider = GetComponent<Collider2D>();
                Collider2D otherCollider = enemy.GetComponent<Collider2D>();

                if (thisCollider != null && otherCollider != null)
                {
                    Physics2D.IgnoreCollision(thisCollider, otherCollider);
                }
            }
        }
    }*/

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
                collision.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public void RecoverAfterPush(float delay)
    {
        if (isDead) return;
        if (gameObject.activeInHierarchy)
            StartCoroutine(RecoverRoutine(delay));
    }

    private IEnumerator RecoverRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isSucked = false; // vuelve a caminar y atacar
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.CheckVictoryConditionAfterEnemyDeath();
    }
}