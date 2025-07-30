using System.Collections;
using UnityEngine;

public class AllyController : MonoBehaviour
{
    private PlayerController playerController;
    public float moveSpeed;
    public float followDistance = 2f;
    public float detectionRadius = 5f;
    public float attackCooldown = 1f;
    public GameObject bulletPrefab;
    public Transform shootingPoint;
    private float fireRate = 1f;
    private float nextFireTime;

    private float smoothSpeed = 0f;
    public float speedLerpFactor = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Transform player;
    private Rigidbody2D allyRigidbody;
    private Animator allyAnimator;
    private float attackTimer = 0f;
    private bool isAllyOnGround;
    private bool facingRight = true;
    public bool isDead = false;

    private readonly int playerSpeedID = Animator.StringToHash("PlayerSpeed");
    private readonly int onGroundID = Animator.StringToHash("OnGround");
    private readonly int isShootingID = Animator.StringToHash("IsShooting");
    private readonly int hurtID = Animator.StringToHash("Hurt");

    public int maxHealth = 3;
    private int currentHealth;

    [Header("Audio")]
    public AudioClip bulletSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {
        nextFireTime = Time.time + Random.Range(0f, 0.5f);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            moveSpeed = playerController.playerSpeed;
        }
        allyRigidbody = GetComponent<Rigidbody2D>();
        allyAnimator = GetComponent<Animator>();

        currentHealth = maxHealth; // Inicializa la vida al máximo
    }

    void Update()
    {
        // BLOQUEO TOTAL de lógica si está muerto, nada de movimiento, ni ataque, ni trigger animaciones de otros métodos
        if (isDead) return;

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate + Random.Range(-0.1f, 0.1f);
        }

        if (player == null) return;

        CheckGroundStatus();
        FollowPlayer();
        DetectAndAttackEnemies();
        LookAtTarget();
    }

    private void CheckGroundStatus()
    {
        if (isDead) return;
        isAllyOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        allyAnimator.SetBool(onGroundID, isAllyOnGround);
    }

    private void FollowPlayer()
    {
        if (isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > followDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float playerSpeed = playerController != null ? playerController.playerSpeed : moveSpeed;
            allyRigidbody.velocity = direction * playerSpeed;

            // Flip después de mover
            if (direction.x > 0 && !facingRight)
                FlipPlayer();
            else if (direction.x < 0 && facingRight)
                FlipPlayer();

            float targetSpeed = allyRigidbody.velocity.magnitude;
            // Suaviza el cambio de velocidad
            smoothSpeed = Mathf.Lerp(smoothSpeed, targetSpeed, Time.deltaTime * speedLerpFactor);
            allyAnimator.SetFloat(playerSpeedID, smoothSpeed);
            bool isWalking = smoothSpeed > 0.1f;
            bool isFiring = attackTimer > 0;
            allyAnimator.SetBool("IsFiringOnWalk", isWalking && isFiring);
            allyAnimator.SetBool(isShootingID, isFiring && !isWalking);
        }
        else
        {
            allyRigidbody.velocity = Vector2.zero;
            smoothSpeed = Mathf.Lerp(smoothSpeed, 0f, Time.deltaTime * speedLerpFactor);
            allyAnimator.SetFloat(playerSpeedID, smoothSpeed);
            allyAnimator.SetBool("IsFiringOnWalk", false);
            if (attackTimer <= 0)
                allyAnimator.SetBool(isShootingID, false);
        }
    }

    private void DetectAndAttackEnemies()
    {
        if (isDead) return;
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = col.transform;
                }
            }
        }

        if (nearestEnemy != null)
        {
            Attack(nearestEnemy);
        }
    }

    private void Attack(Transform target)
    {
        if (isDead) return;
        if (bulletPrefab != null && shootingPoint != null)
        {
            allyAnimator.SetTrigger(isShootingID);

            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
            Vector2 direction = (target.position - shootingPoint.position).normalized;
            bullet.GetComponent<Rigidbody2D>().velocity = direction * moveSpeed * 2;

            // Orientar al aliado hacia el enemigo
            if (target.position.x > transform.position.x && !facingRight)
            {
                FlipPlayer();
            }
            else if (target.position.x < transform.position.x && facingRight)
            {
                FlipPlayer();
            }

            // Rotar la bala para que apunte en la dirección correcta
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Reproducir sonido de disparo
            if (bulletSound != null && audioSource != null)
                audioSource.PlayOneShot(bulletSound);

            attackTimer = attackCooldown + Random.Range(-0.3f, 0.3f);
        }
    }

    private void LookAtTarget()
    {
        if (isDead) return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = col.transform;
                }
            }
        }

        if (nearestEnemy != null)
        {
            // Mirar al enemigo
            if (nearestEnemy.position.x > transform.position.x && !facingRight)
            {
                FlipPlayer();
            }
            else if (nearestEnemy.position.x < transform.position.x && facingRight)
            {
                FlipPlayer();
            }
        }
        else
        {
            // Mirar al jugador
            if (player.position.x > transform.position.x && !facingRight)
            {
                FlipPlayer();
            }
            else if (player.position.x < transform.position.x && facingRight)
            {
                FlipPlayer();
            }
        }
    }

    private void FlipPlayer()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("[Ally] Recibe daño, vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (allyAnimator != null)
            {
                allyAnimator.SetTrigger(hurtID);
                Debug.Log("[Ally] Trigger Hurt activado por daño NO mortal");
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("[Ally] Muere. Forzando animación Hurt y bloqueando más daño");
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        if (allyAnimator != null)
        {
            allyAnimator.ResetTrigger(isShootingID);
            allyAnimator.ResetTrigger(hurtID);
            allyAnimator.SetBool("IsFiringOnWalk", false);
            allyAnimator.SetBool(isShootingID, false);
            allyAnimator.SetFloat(playerSpeedID, 0);
            allyAnimator.Play("Hurt", 0, 0f);
            Debug.Log("[Ally] Estado Hurt forzado con Play()");
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        allyRigidbody.velocity = Vector2.zero;
        this.enabled = false;
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        Debug.Log("[Ally] Esperando para destruir tras animación Hurt");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("[Ally] ¡Ahora se destruye!");
        Destroy(gameObject);
    }

}