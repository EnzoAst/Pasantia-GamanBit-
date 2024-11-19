using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// Definimos la clase PlayerController que hereda de MonoBehaviour
public class PlayerController : MonoBehaviour
{
    // Instancia estática para implementar el patrón Singleton
    public static PlayerController Instance;

    // Variables para el salto
    public float jumpForce = 10f;                // Fuerza base del salto
    public float airJumpForceMultiplier = 0.5f;  // Multiplicador de fuerza cuando está en el aire
    private Rigidbody2D rb;                      // Referencia al componente Rigidbody2D
    private bool enPlaneta = false;              // Indica si el jugador está sobre un planeta
    private Transform currentPlanet;             // Referencia al planeta actual

    public float planetRadiusOffset = 0.5f;      // Offset para la posición en el planeta

    private Vector3 posicionInicial;             // Posición inicial del jugador

    // Elementos de la interfaz de usuario
    public TextMeshProUGUI puntosTexto;          // Texto para mostrar los puntos
    public Image[] corazones;                    // Arreglo de imágenes para representar las vidas

    private Animator animator;                   // Referencia al componente Animator

    private const string SCORE_KEY = "PlayerScore";  // Clave para guardar y cargar el puntaje

    private AudioSource audioSource;             // Fuente de audio para reproducir sonidos
    public AudioClip sonidoPerderVida;           // Clip de audio al perder una vida

    // Variables para la atracción gravitacional
    public float gravityForce = 5f;              // Fuerza de atracción hacia el planeta más cercano
    private GameObject[] planets;                // Arreglo para almacenar todos los planetas en la escena
    public float maxSpeed = 10f;                 // Velocidad máxima permitida para el jugador

    // Variable para el sonido de salto
    public AudioClip sonidoSalto;                // Clip de audio para el salto

    // Variable para controlar si el jugador está en proceso de respawn o game over
    private bool isProcessing = false;

    private SpriteRenderer spriteRenderer;       // Referencia al SpriteRenderer para el efecto de parpadeo

    private void Awake()
    {
        // Asignamos esta instancia a la variable estática Instance
        Instance = this;
    }

    public void ReduceLife()
    {
        // Si ya estamos procesando un respawn o game over, no hacemos nada
        if (isProcessing)
            return;

        // Restamos una vida al jugador
        GameController.Instance.RestarVida();
        Debug.Log("Vidas restantes: " + GameController.Instance.vidas);

        if (GameController.Instance.vidas > 0)
        {
            // Reproducimos el sonido de perder vida
            audioSource.PlayOneShot(sonidoPerderVida);

            // Iniciamos la corrutina para manejar el respawn del jugador
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            // Iniciamos la corrutina para manejar el game over
            StartCoroutine(HandleGameOver());
        }
    }

    // Corrutina para manejar el respawn del jugador
    private IEnumerator RespawnPlayer()
    {
        isProcessing = true;

        // Detenemos movimiento, físicas y animaciones
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;             // Reseteamos la velocidad angular
        rb.isKinematic = true;               // Evitamos que las físicas afecten al jugador
        animator.enabled = false;            // Desactivamos las animaciones
        enPlaneta = false;                   // Desvinculamos del estado en planeta
        transform.SetParent(null);           // Aseguramos que no tenga un padre (ningún planeta)

        // Deshabilitamos el collider para evitar colisiones durante el respawn
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Reproducimos el efecto de parpadeo durante 2 segundos
        yield return StartCoroutine(BlinkPlayer(2f, 0.1f));

        // Reiniciamos la posición y estado del jugador
        transform.position = posicionInicial;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Rehabilitamos controles, físicas y animaciones
        rb.isKinematic = false;
        animator.enabled = true;
        if (collider != null)
        {
            collider.enabled = true;
        }

        isProcessing = false;

        // Actualizamos la interfaz de usuario
        UpdateUI();
    }


    // Corrutina para manejar el game over
    private IEnumerator HandleGameOver()
    {
        isProcessing = true;

        // Deshabilitamos controles y animaciones
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        animator.enabled = false;

        // Deshabilitamos el collider para evitar colisiones
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Reproducimos el sonido de perder vida
        audioSource.PlayOneShot(sonidoPerderVida);

        // Reproducimos el efecto de parpadeo durante 2 segundos
        yield return StartCoroutine(BlinkPlayer(2f, 0.1f));

        // Esperamos a que el sonido termine de reproducirse
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // Mostramos la escena de Game Over
        MostrarGameOver();
    }

    // Corrutina para el efecto de parpadeo del jugador
    private IEnumerator BlinkPlayer(float duration, float blinkTime)
    {
        float elapsedTime = 0f;   // Tiempo transcurrido
        bool isVisible = true;    // Estado de visibilidad del sprite

        while (elapsedTime < duration)
        {
            isVisible = !isVisible;                 // Alternamos la visibilidad
            spriteRenderer.enabled = isVisible;     // Aplicamos el cambio al SpriteRenderer

            elapsedTime += blinkTime;               // Incrementamos el tiempo transcurrido
            yield return new WaitForSeconds(blinkTime);  // Esperamos el tiempo de parpadeo
        }

        // Aseguramos que el sprite esté visible al finalizar
        spriteRenderer.enabled = true;
    }

    void Start()
    {
        // Obtenemos referencias a los componentes necesarios
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;                     // Desactivamos la gravedad por defecto
        posicionInicial = transform.position;    // Guardamos la posición inicial del jugador
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Para el efecto de parpadeo

        // Obtenemos todos los planetas en la escena con la etiqueta "Planet"
        planets = GameObject.FindGameObjectsWithTag("Planet");

        // Cargamos el puntaje guardado si existe
        if (PlayerPrefs.HasKey(SCORE_KEY))
        {
            GameController.Instance.puntos = PlayerPrefs.GetInt(SCORE_KEY);
        }
        else
        {
            GameController.Instance.puntos = 0;
        }

        // Actualizamos la interfaz de usuario
        UpdateUI();
    }

    void Update()
    {
        // Si estamos procesando un respawn o game over, no hacemos nada
        if (isProcessing)
            return;

        // Detectamos si el jugador presiona la tecla de salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();  // Ejecutamos el salto
        }

        // Si el jugador está en un planeta y tiene un planeta actual asignado
        if (enPlaneta && currentPlanet != null)
        {
            // Calculamos la dirección hacia el planeta
            Vector3 directionToPlanet = (transform.position - currentPlanet.position).normalized;

            // Mantenemos al jugador en la superficie del planeta
            transform.position = currentPlanet.position + directionToPlanet * (currentPlanet.localScale.x / 2);

            // Orientamos al jugador perpendicularmente a la superficie del planeta
            Vector3 gravityDirection = (currentPlanet.position - transform.position).normalized;
            float angle = Mathf.Atan2(gravityDirection.y, gravityDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
        else
        {
            // Atraemos al jugador hacia el planeta más cercano
            AttractToNearestPlanet();
        }

        // Manejamos las animaciones de movimiento
        HandleMovementAnimations();

        // Actualizamos la interfaz de usuario
        UpdateUI();
    }

    void FixedUpdate()
    {
        // Si estamos procesando un respawn o game over, no hacemos nada
        if (isProcessing)
            return;

        // Limitamos la velocidad máxima del jugador
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void Jump()
    {
        // Si estamos procesando un respawn o game over, no hacemos nada
        if (isProcessing)
            return;

        enPlaneta = false;            // Indicamos que el jugador ya no está en un planeta
        transform.SetParent(null);    // Desvinculamos al jugador del planeta

        // Determinamos la dirección del salto según las teclas presionadas
        Vector2 jumpDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) jumpDirection = Vector2.left;
        if (Input.GetKey(KeyCode.D)) jumpDirection = Vector2.right;
        if (Input.GetKey(KeyCode.W)) jumpDirection = Vector2.up;
        if (Input.GetKey(KeyCode.S)) jumpDirection = Vector2.down;

        if (jumpDirection == Vector2.zero) jumpDirection = Vector2.up; // Salto vertical por defecto

        rb.velocity = Vector2.zero;   // Reiniciamos la velocidad actual

        // Calculamos la fuerza de salto aplicada
        float appliedJumpForce = enPlaneta ? jumpForce : jumpForce * airJumpForceMultiplier;

        // Aplicamos la fuerza de salto en la dirección determinada
        rb.AddForce(jumpDirection * appliedJumpForce, ForceMode2D.Impulse);

        // Reproducimos el sonido de salto
        audioSource.PlayOneShot(sonidoSalto);

        // Activamos la animación de salto
        animator.SetBool("isJumping", true);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si estamos procesando un respawn o game over, no hacemos nada
        if (isProcessing)
            return;

        Debug.Log("Colisión con: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Planet"))
        {
            // El jugador ha aterrizado en un planeta
            enPlaneta = true;
            rb.velocity = Vector2.zero;          // Detenemos el movimiento
            currentPlanet = collision.transform; // Actualizamos el planeta actual
            transform.SetParent(currentPlanet);  // Vinculamos al jugador al planeta

            // Detenemos cualquier rotación residual
            rb.angularVelocity = 0f;
            rb.rotation = 0f;

            // Desactivamos la animación de salto
            animator.SetBool("isJumping", false);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // El jugador ha chocado con un enemigo
            Debug.Log("Has tocado un enemigo");
            ReduceLife();  // Reducimos una vida
        }
        else if (collision.gameObject.CompareTag("WinCondition"))
        {
            // El jugador ha alcanzado la condición de victoria
            MostrarCongratulationsScene();  // Mostramos la escena de felicitaciones
        }
    }

    // Método para atraer al jugador hacia el planeta más cercano
    void AttractToNearestPlanet()
    {
        // Si estamos procesando un respawn o game over, no hacemos nada
        if (isProcessing || planets.Length == 0) return;

        Transform nearestPlanet = null;
        float minDistance = Mathf.Infinity;

        // Buscamos el planeta más cercano
        foreach (GameObject planet in planets)
        {
            float distance = Vector2.Distance(transform.position, planet.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlanet = planet.transform;
            }
        }

        // Si encontramos un planeta cercano
        if (nearestPlanet != null)
        {
            // Calculamos la dirección hacia el planeta más cercano
            Vector2 direction = (nearestPlanet.position - transform.position).normalized;

            // Aplicamos la fuerza de atracción gravitacional
            rb.AddForce(direction * gravityForce * Time.deltaTime, ForceMode2D.Force);
        }
    }


    // Método para obtener la posición inicial del jugador
    public Vector3 GetInitialPosition()
    {
        return posicionInicial;
    }

    // Método para sumar puntos al puntaje del jugador
    public void SumarPuntos(int cantidad)
    {
        GameController.Instance.SumarPuntos(cantidad);
        UpdateUI();  // Actualizamos la interfaz de usuario
    }

    // Método para manejar las animaciones de movimiento
    void HandleMovementAnimations()
    {
        // Si estamos procesando un respawn o game over, no hacemos nada
        if (isProcessing)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0 && enPlaneta)
        {
            // Activamos la animación de caminar
            animator.SetBool("isWalking", true);
        }
        else
        {
            // Desactivamos la animación de caminar
            animator.SetBool("isWalking", false);
        }
    }

    // Método para mostrar la escena de Game Over
    public void MostrarGameOver()
    {
        // Guardamos el puntaje y las vidas en PlayerPrefs
        PlayerPrefs.SetInt(SCORE_KEY, GameController.Instance.puntos);
        PlayerPrefs.SetInt("PlayerLives", 0);

        // Intentamos encontrar el GameOverManager en la escena
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
        {
            // Llamamos al método MostrarGameOver() del GameOverManager
            gameOverManager.MostrarGameOver();
        }
        else
        {
            Debug.LogError("No se encontró GameOverManager en la escena.");
            // Como respaldo, cargamos la escena de Game Over después de un retardo
            StartCoroutine(RetardoCargaGameOver());
        }
    }

    // Corrutina de respaldo para cargar la escena de Game Over después de un retardo
    private IEnumerator RetardoCargaGameOver()
    {
        // Esperamos 2 segundos antes de cargar la escena
        yield return new WaitForSeconds(1f);

        // Cargamos la escena de Game Over
        SceneManager.LoadScene("GameOverScene");
    }

    // Método para mostrar la escena de felicitaciones
    void MostrarCongratulationsScene()
    {
        // Guardamos el puntaje en PlayerPrefs
        PlayerPrefs.SetInt(SCORE_KEY, GameController.Instance.puntos);

        // Cargamos la escena de felicitaciones
        SceneManager.LoadScene("Congratulations");
    }

    // Método para actualizar la interfaz de usuario
    void UpdateUI()
    {
        // Actualizamos el texto del puntaje
        puntosTexto.text = "Score " + GameController.Instance.puntos;

        // Actualizamos las imágenes de los corazones según las vidas restantes
        for (int i = 0; i < corazones.Length; i++)
        {
            if (i < GameController.Instance.vidas)
            {
                corazones[i].enabled = true;  // Mostramos el corazón
            }
            else
            {
                corazones[i].enabled = false; // Ocultamos el corazón
            }
        }
    }
}