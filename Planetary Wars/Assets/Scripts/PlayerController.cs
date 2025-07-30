using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    public bool isJumping = false;
    private bool isShooting = false;
    private bool isPlayingJumpAnim = false;
    private float originalSpeed;

    public Transform shootingPoint;
    public GameObject blackHoleObject;
    public GameObject waveAbilityPrefab; // Prefab de la habilidad de onda expansiva
    public float waveCooldown = 10f; // Tiempo de reutilización de la habilidad en segundos
    public float lastWaveTime; // Momento en el que se usó la habilidad por última vez
    public float cooldownTime = 10f; // Tiempo de cooldown
    public float nextAvailableTime = 0f; // Momento en que el botón puede ser presionado nuevamente
    public bool isCooldown = false; // Indica si está en cooldown
    public float hurtCounter;
    public float shootingCounter;

    public float specialBulletSpeed = 5f; // Velocidad reducida para que se mueva lento
    public Vector3 specialBulletScale = new Vector3(2f, 2f, 1f); // Tamaño mayor para el disparo especial

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    private Rigidbody2D playerRigidbody;
    private Animator playerAnimator;
    private bool isPlayerOnGround;
    private float vHurtCounter;
    private float vShootingCounter;
    private bool facingRight;
    public int bulletsAmount = 10;
    private int bulletIndex;
    private WaitForSeconds wait;

    private bool isDead = false; // Bloquea las acciones al morir

    public Transform shadow; // Referencia a la sombra
    private float shadowFixedY; // Guarda la posición Y de la sombra cuando el jugador salta

    // Nuevo: radio de detección de enemigos para disparo automático
    public float enemyDetectionRadius = 5f;

    [Header("Audio")]
    public AudioClip bulletSound;
    public AudioClip blackHoleSound;
    public AudioClip waveSound;
    public AudioClip jumpSound;
    private AudioSource audioSource;

    [Header("Turret Settings")]
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private float turretOffset = 2f;
    [SerializeField] public float turretCooldown = 5f;  // <-- Cooldown en segundos

    public float nextTurretTime = 0f;  // Tiempo hasta que pueda volver a colocar una torreta

    private readonly int playerSpeedID = Animator.StringToHash("PlayerSpeed");
    private readonly int onGroundID = Animator.StringToHash("OnGround");
    private readonly int jumpID = Animator.StringToHash("Jump");

    [Header("Gravity Settings")]
    public float baseSpeed = 6f;
    public float baseJumpHeight = 2f;
    public float baseJumpDuration = 0.5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        facingRight = true;
        wait = new WaitForSeconds(1.5f);
        playerSpeed = SceneGravity.playerSpeed;
        jumpHeight = SceneGravity.jumpHeight;
        jumpDuration = SceneGravity.jumpDuration;

        // Configuramos el pool de balas (asegúrate que BulletPool ya exista en la escena)
        BulletPool.bulletPoolInstance.totalBulletsInPool = bulletsAmount;

        // Desactivamos el BlackHole al inicio
        if (blackHoleObject != null)
            blackHoleObject.SetActive(false);

        lastWaveTime = -waveCooldown;

        IgnoreCollisionsWithEnemiesAndAllies();

        if (shadow != null)
        {
            shadow.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            shadowFixedY = shadow.position.y; // Asegura que inicie en la posición correcta
        }
    }

    void Update()
    {
        if (isDead) return;

        if (!isPlayingJumpAnim)
        {
            playerAnimator.SetFloat(playerSpeedID, playerRigidbody.velocity.magnitude);
        }
        else
        {
            playerAnimator.SetFloat(playerSpeedID, 0);
        }

        bool isPlayerOnGround = !isJumping;
        playerAnimator.SetBool(onGroundID, isPlayerOnGround);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool isWalking = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;

        bool isPressingShoot = vShootingCounter > 0f;

        if (SkillPanelController.IsSkillPanelOpen)
            return;

        // --- Animaciones de disparo y salto ---
        if (!isPlayerOnGround && isPressingShoot)
        {
            playerAnimator.SetBool("IsShooting", false);
            playerAnimator.SetBool("IsFiringOnWalk", false);
            playerAnimator.SetTrigger("ShootOnAir");
        }
        else if (isPlayerOnGround && isPressingShoot && isWalking)
        {
            playerAnimator.SetBool("IsFiringOnWalk", true);
            playerAnimator.SetBool("IsShooting", false);
        }
        else if (isPlayerOnGround && isPressingShoot)
        {
            playerAnimator.SetBool("IsFiringOnWalk", false);
            playerAnimator.SetBool("IsShooting", true);
        }
        else
        {
            playerAnimator.SetBool("IsFiringOnWalk", false);
            playerAnimator.SetBool("IsShooting", false);
        }

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)) && !isJumping)
        {
            playerAnimator.SetTrigger(jumpID);
            StartCoroutine(Jump());
        }

        if (vHurtCounter <= 0f)
        {
            Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * playerSpeed;
            playerRigidbody.velocity = new Vector2(movement.x, movement.y);

            if (horizontalInput > 0 && !facingRight)
            {
                FlipPlayer();
            }
            else if (horizontalInput < 0 && facingRight)
            {
                FlipPlayer();
            }
        }
        else
        {
            vHurtCounter -= Time.deltaTime;
        }

        playerAnimator.SetFloat(playerSpeedID, playerRigidbody.velocity.magnitude);

        if (vShootingCounter <= 0f)
        {
            isShooting = false;
        }
        else
        {
            vShootingCounter -= Time.deltaTime;
        }

        UpdateShadowPosition();
        AutoShootIfEnemyNearby();
        FaceNearestEnemy();

        if (!PauseMenu.escapeEnabled ||
            (GameManager.instance != null &&
             (GameManager.instance.IsVictoryPanelActive() || GameManager.instance.IsDefeatPanelActive())) ||
            (FindObjectOfType<PauseMenu>() != null && FindObjectOfType<PauseMenu>().pauseMenuUI.activeSelf))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextAvailableTime)
        {
            ActivateBlackHoleSkill();
            nextAvailableTime = Time.time + cooldownTime;
            isCooldown = true;
        }

        if (Time.time >= nextAvailableTime)
        {
            isCooldown = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastWaveTime + waveCooldown)
        {
            ActivateWaveAbility();
        }

        if (Input.GetKeyDown(KeyCode.T) && Time.time >= nextTurretTime)
        {
            SpawnTurret();
            nextTurretTime = Time.time + turretCooldown;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<UltimateMissileAbility>().Activate();
        }
    }

    public void ResetSpeed()
    {
        playerSpeed = originalSpeed;
    }
    void ActivateWaveAbility()
    {
        // Instanciar el prefab de la habilidad en la posición del jugador
        GameObject waveAbility = Instantiate(waveAbilityPrefab, transform.position, Quaternion.identity);

        // Activar la onda expansiva
        waveAbility.GetComponent<WaveAbility>().ActivateWave();

        audioSource.PlayOneShot(waveSound); // SONIDO de la onda

        // Registrar el momento en que se usó la habilidad
        lastWaveTime = Time.time;
    }

    void IgnoreCollisionsWithEnemiesAndAllies()
    {
        Collider2D myCollider = GetComponent<Collider2D>();

        foreach (var tag in new string[] { "Enemy", "Boss", "Ally" })
        {
            GameObject[] others = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in others)
            {
                Collider2D otherCol = obj.GetComponent<Collider2D>();
                if (otherCol != null && myCollider != null)
                    Physics2D.IgnoreCollision(myCollider, otherCol);
            }
        }
    }

    private void SpawnTurret()
    {
        // Posición en la que se colocará la torreta (delante del jugador)
        Vector3 spawnPosition = transform.position + transform.up * turretOffset;

        // Instanciar la torreta en la posición calculada
        Instantiate(turretPrefab, spawnPosition, Quaternion.identity);
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        isPlayingJumpAnim = true;
        playerAnimator.SetTrigger(jumpID);
        PlayJumpSound();

        // Si NO está disparando, animación de salto normal
        if (!isShooting)
            playerAnimator.SetTrigger("Jump");
        // Si está disparando, la animación de disparo en aire ya se maneja en Shoot

        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        shadowFixedY = shadow.position.y;

        while (elapsedTime < jumpDuration)
        {
            float progress = elapsedTime / jumpDuration;
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            float horizontalMovement = horizontalInput * playerSpeed * elapsedTime;

            transform.position = new Vector2(
                startPosition.x + horizontalMovement,
                startPosition.y + height
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector2(transform.position.x, startPosition.y);
        isJumping = false;
        isPlayingJumpAnim = false;
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    public bool IsPlayerOnGround()
    {
        return isPlayerOnGround;
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    private void FlipPlayer()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void UpdateShadowPosition()
    {
        if (shadow != null)
        {
            if (isJumping)
            {
                // Mientras salta, la sombra sigue en X y Z pero mantiene su Y
                shadow.position = new Vector3(transform.position.x, shadowFixedY, transform.position.z);
            }
            else
            {
                // Cuando aterriza, la sombra sigue completamente al jugador
                shadow.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            }
        }
    }

    // --- Método modificado para detectar enemigos y calcular la dirección de disparo ---
    private void AutoShootIfEnemyNearby()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, enemyDetectionRadius);
        Transform targetEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetEnemy = col.transform;
                }
            }
        }

        if (targetEnemy != null && vShootingCounter <= 0f)
        {
            Shoot(targetEnemy);
        }
    }

    private void Shoot(Transform targetEnemy)
    {
        // Si el jugador está en el aire, dispara la animación correspondiente
        if (!isJumping)
        {
            // En el suelo: solo dependemos del parámetro IsShooting en Update
            // NO ponemos trigger aquí, solo responsabilidades de disparo
        }
        else
        {
            // En el aire: trigger de disparo en el aire
            playerAnimator.SetTrigger("ShootOnAir");
        }

        GameObject bullet = BulletPool.bulletPoolInstance.GetBullet(bulletIndex);
        if (bullet != null)
        {
            bullet.transform.position = shootingPoint.position;
            Vector2 direction = (targetEnemy.position - shootingPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            bullet.SetActive(true);

            // SONIDO de disparo
            audioSource.PlayOneShot(bulletSound);
        }

        bulletIndex = (bulletIndex + 1) % bulletsAmount;
        vShootingCounter = shootingCounter;
    }

    private void ActivateBlackHoleSkill()
    {
        // Verifica si el cooldown ha pasado
        if (Time.time < nextAvailableTime)
        {
            Debug.Log("BlackHoleSkill está en cooldown. Espera para usarlo.");
            return; // Sale de la función si está en cooldown
        }

        // Activa la habilidad si no está en cooldown
        Debug.Log("Se presionó F: Activando BlackHoleSkill");
        playerAnimator.SetTrigger("SkillAttack");

        if (blackHoleObject != null)
        {
            // Instancia el prefab del BlackHoleSkill en la posición deseada (por ejemplo, en shootingPoint)
            GameObject bhInstance = Instantiate(blackHoleObject, shootingPoint.position, Quaternion.identity);

            // Si la instancia se crea inactiva, la forzamos a estar activa
            if (!bhInstance.activeSelf)
            {
                bhInstance.SetActive(true);
            }

            Debug.Log("BlackHole instanciado y activado: " + bhInstance.name);

            // Establece el tiempo para el próximo uso de la habilidad
            nextAvailableTime = Time.time + cooldownTime;
            audioSource.PlayOneShot(blackHoleSound); // SONIDO del Black Hole
        }
        else
        {
            Debug.LogWarning("El prefab 'blackHoleObject' no está asignado en el inspector.");
        }
    }

    // Opcional: Dibuja en el editor la esfera de detección de enemigos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectionRadius);
    }

    // Método para orientar al jugador hacia el enemigo más cercano
    private void FaceNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, enemyDetectionRadius);
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        // Buscar el enemigo más cercano dentro del radio
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = col.transform;
                }
            }
        }

        // Si se encontró un enemigo, orienta al jugador según su posición
        if (nearestEnemy != null)
        {
            if (nearestEnemy.position.x > transform.position.x && !facingRight)
            {
                FlipPlayer();
            }
            else if (nearestEnemy.position.x < transform.position.x && facingRight)
            {
                FlipPlayer();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AllyItem"))
        {
            // Llamar al AllySpawner para generar aliados
            AllySpawner allySpawner = FindObjectOfType<AllySpawner>();
            if (allySpawner != null)
            {
                allySpawner.SpawnAllies(1);
            }

            Destroy(collision.gameObject); // Destruir el ítem recolectado
        }
    }

    public void TakeDamage(int damage)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.TakeDamage(damage); // Llama al método del GameManager
        }

        // Aquí puedes agregar efectos visuales o sonidos al recibir daño
        Debug.Log("¡El jugador ha recibido daño!");
    }

    public void Die()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Hurt"); // Activar animación de muerte
        }

        isDead = true; // Bloquear acciones
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.isKinematic = true; // Detener físicas

        Time.timeScale = 0f; // Congelar el tiempo

        StartCoroutine(WaitForDeathAnimation()); // Esperar la animación
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSecondsRealtime(2f); // Esperar mientras el tiempo está congelado

        Time.timeScale = 1f; // Descongelar el tiempo
        GameManager.instance.RestartGame(); // Reiniciar el juego
    }
}