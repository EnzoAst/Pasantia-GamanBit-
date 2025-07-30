using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Stats")]
    public int playerLife = 100;
    public int playerScore = 0;
    private int originalInitialLife;

    [Header("Configuración del Jugador")]
    public int initialLife = 100;

    [Header("Special Attack Control")]
    private int scoreThreshold = 1000; // Puntos necesarios para desbloquear el ataque especial
    private int timesSpecialActivated = 0; // Cantidad de veces que se activó el ataque especial
    public bool isSpecialAttackAvailable = false; // Indica si el ataque especial está disponible

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI skillsLifeText;
    public GameObject remainingEnemiesMessage;
    private CanvasGroup remainingEnemiesGroup;

    [Header("Puntaje Global")]
    public int totalScore = 0;

    [Header("Victory UI")]
    [SerializeField] public GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryLevelScoreText;
    [SerializeField] private TextMeshProUGUI victoryTotalScoreText;

    [Header("Defeat UI")]
    [SerializeField] public GameObject defeatPanel;
    [SerializeField] private TextMeshProUGUI defeatLevelScoreText;
    [SerializeField] private TextMeshProUGUI defeatTotalScoreText;

    [Header("Retry Settings")]
    public int maxRetries = 3;
    private int currentRetries = 0;

    [Header("ButtonPause")]
    [SerializeField] public GameObject pauseButton;

    [Header("RetryButton")]
    public UnityEngine.UI.Button retryButton;
    [Header("BackToMenuButton")]
    public UnityEngine.UI.Button backToMenuButton;

    private int lastUpgradeScore = 0; // Agrega esta variable para llevar el control
    private bool hasShownRemainingEnemiesMessage = false;


    public delegate void PortalsChanged(int remaining, int total);
    public event PortalsChanged OnPortalsChanged;
    private int totalPortals = 0;
    public int TotalPortals => totalPortals;
    public int PortalsDestroyed => totalPortals - remainingPortals;
    private int remainingPortals = 0;

    [SerializeField] private UpgradeManager upgradeManager; // Referencia al UpgradeManager (asignar en el Inspector)



    [System.Serializable]
    public class LevelGoal
    {
        public int sceneIndex;
        public int requiredScore;
    }

    [Header("Level Goals")]
    public LevelGoal[] levelGoals;

    void Start()
    {
        // Solo si el objeto tiene que volver a encontrar el botón después del reinicio
        retryButton = GameObject.Find("RetryButton")?.GetComponent<Button>();
        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(RestartGame);
        }

        backToMenuButton = GameObject.Find("BackToMenuButton")?.GetComponent<Button>();
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(BackToMenu);
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Hace que el GameManager persista entre escenas
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);  // Destruye cualquier duplicado del GameManager
        }
        originalInitialLife = initialLife;

        if (remainingEnemiesMessage != null)
            remainingEnemiesGroup = remainingEnemiesMessage.GetComponent<CanvasGroup>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si el GameManager ha sido destruido, salir del método
        if (this == null)
        {
            Debug.LogWarning("OnSceneLoaded fue llamado en un GameManager destruido. Abortando.");
            return;
        }

        Debug.Log($"Escena cargada: {scene.name} - GameManager activo: {gameObject.activeSelf}");

        totalPortals = 4;
        remainingPortals = 4;
        OnPortalsChanged?.Invoke(remainingPortals, totalPortals);

        remainingEnemiesMessage = FindInactiveObjectByTag("EnemyMessage");
        if (remainingEnemiesMessage != null)
            remainingEnemiesGroup = remainingEnemiesMessage.GetComponent<CanvasGroup>();
        else
            remainingEnemiesGroup = null;

        if (scene.name == "Lava")
        {
            PlayerPrefs.SetInt("LastLevelCompleted", 1);
            PlayerPrefs.Save();
            Debug.Log("Jugador entró a 'Lava'. Niveles desbloqueados.");
        }

        if (scene.name == "MainMenu")
        {
            // Encuentra y desactiva los scripts innecesarios
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null) healthBar.enabled = false;

            EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner != null) enemySpawner.enabled = false;
        }

        // UpgradeManager esté asignado
        if (upgradeManager == null)
        {
            upgradeManager = FindObjectOfType<UpgradeManager>();
            if (upgradeManager != null)
            {
                Debug.Log("UpgradeManager asignado correctamente.");
            }
            else
            {
                Debug.LogWarning("No se encontró un UpgradeManager en la escena.");
            }
        }
        // Restaurar vida original
        initialLife = originalInitialLife;
        playerLife = initialLife;

        // Restaurar velocidad original del jugador
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
            player.ResetSpeed();
        playerScore = 0;
        lastUpgradeScore = 0;

        // Reiniciar variables relacionadas con el ataque especial
        isSpecialAttackAvailable = false; // Reinicia la disponibilidad del ataque especial
        timesSpecialActivated = 0;       // Reinicia el contador de activaciones

        CheckSpecialAttackAvailability();

        if (scene.name == "MainMenu")
        {
            totalScore = 0;
            currentRetries = 0;
        }

        // Verifica que el puntaje sea 0 al cargar un nivel
        Debug.Log("Puntaje inicial del jugador: " + playerScore);

        if (pauseButton == null)
            pauseButton = GameObject.Find("ButtonPause");

        // Buscar paneles inactivos
        if (defeatPanel == null)
            defeatPanel = FindInactiveObjectByName("DefeatPanel");

        if (victoryPanel == null)
            victoryPanel = FindInactiveObjectByName("VictoryPanel");

        // Buscar textos visibles (activos)
        if (scoreText == null)
            scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TextMeshProUGUI>();

        if (lifeText == null)
            lifeText = GameObject.FindWithTag("LifeText")?.GetComponent<TextMeshProUGUI>();

        // Buscar textos dentro de paneles inactivos
        if (victoryLevelScoreText == null)
        {
            GameObject obj = FindInactiveObjectByName("VictoryLevelScore");
            if (obj != null) victoryLevelScoreText = obj.GetComponent<TextMeshProUGUI>();
        }

        if (victoryTotalScoreText == null)
        {
            GameObject obj = FindInactiveObjectByName("VictoryTotalScore");
            if (obj != null) victoryTotalScoreText = obj.GetComponent<TextMeshProUGUI>();
        }

        if (defeatLevelScoreText == null)
        {
            GameObject obj = FindInactiveObjectByName("DefeatLevelScore");
            if (obj != null) defeatLevelScoreText = obj.GetComponent<TextMeshProUGUI>();
        }

        if (defeatTotalScoreText == null)
        {
            GameObject obj = FindInactiveObjectByName("DefeatTotalScore");
            if (obj != null) defeatTotalScoreText = obj.GetComponent<TextMeshProUGUI>();
        }

        // Buscar el DefeatPanel y sus botones
        if (defeatPanel == null)
            defeatPanel = FindInactiveObjectByName("DefeatPanel");

        if (defeatPanel != null)
        {
            // Buscar RetryButton dentro del DefeatPanel
            GameObject objRetry = defeatPanel.transform.Find("RetryButton")?.gameObject;
            if (objRetry != null)
            {
                retryButton = objRetry.GetComponent<Button>();
                if (retryButton != null)
                {
                    retryButton.onClick.RemoveAllListeners();
                    retryButton.onClick.AddListener(RestartGame); // Asigna el método dinámicamente
                    Debug.Log("RetryButton reasignado correctamente.");
                }
            }

            // Buscar BackToMenuButton dentro del DefeatPanel
            GameObject objBackToMenu = defeatPanel.transform.Find("BackToMenuButton")?.gameObject;
            if (objBackToMenu != null)
            {
                backToMenuButton = objBackToMenu.GetComponent<Button>();
                if (backToMenuButton != null)
                {
                    backToMenuButton.onClick.RemoveAllListeners();
                    backToMenuButton.onClick.AddListener(BackToMenu); // Asigna el método dinámicamente
                    Debug.Log("BackToMenuButton reasignado correctamente.");
                }
            }
            // Buscar BackToMenuButton dentro del VictoryPanel
            GameObject objBackToMenuVictory = victoryPanel.transform.Find("BackToMenuButton")?.gameObject;
            if (objBackToMenuVictory != null)
            {
                var backToMenuVictoryButton = objBackToMenuVictory.GetComponent<Button>();
                if (backToMenuVictoryButton != null)
                {
                    backToMenuVictoryButton.onClick.RemoveAllListeners();
                    backToMenuVictoryButton.onClick.AddListener(BackToMenu); // Asigna el método dinámicamente
                    Debug.Log("BackToMenuButton del VictoryPanel reasignado correctamente.");
                }
            }
        }

        // Botón NextLevel dentro de panel inactivo
        GameObject nextLevelObj = FindInactiveObjectByName("NextLevelButton");
        if (nextLevelObj != null)
        {
            var nextButton = nextLevelObj.GetComponent<UnityEngine.UI.Button>();
            if (nextButton != null)
                nextButton.onClick.AddListener(NextLevel); // Asegura que tenga la función asignada
        }

        // Desactivar paneles por si se cargaron activos
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Resetear tiempo y actualizar HUD
        Time.timeScale = 1f;
        UpdateHUD();
        hasShownRemainingEnemiesMessage = false;
        PauseMenu.escapeEnabled = true;
    }


    private GameObject FindInactiveObjectByName(string name)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.name == name && t.gameObject.scene.isLoaded)
            {
                Debug.Log($"Objeto encontrado: {name}");
                return t.gameObject;
            }
        }

        Debug.LogWarning($"No se encontró un objeto con el nombre: {name}");
        return null; // Retorna null si no se encuentra
    }


    public void NextLevel()
    {
        // Reinicia la vida y el puntaje del nivel actual
        playerLife = initialLife; // Usa la vida inicial configurada
        playerScore = 0; // Reinicia el puntaje del nivel actual
        lastUpgradeScore = 0;
        UpdateHUD(); // Actualiza la UI antes de cargar el siguiente nivel

        // Calcula el índice del siguiente nivel
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Si hay más niveles, cárgalos; si no, regresa al menú principal
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Juego terminado. Puntaje total: " + totalScore);
            SceneManager.LoadScene(0); // Regresa al menú principal
        }

        // Reinicia el contador de reintentos
        currentRetries = 0;
        UpdateRetryButton();
    }

    public void AddScore(int points)
    {
        playerScore += points;

        // Verificar si se desbloquea el ataque especial
        CheckSpecialAttackAvailability();

        if (scoreText == null)
            scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TextMeshProUGUI>();

        UpdateScoreUI();

        if (playerScore - lastUpgradeScore >= 500 && upgradeManager != null)
        {
            if (pauseButton != null)
                pauseButton.SetActive(false);
            upgradeManager.ShowUpgradePanel();
            lastUpgradeScore = playerScore;
        }

        if (playerScore >= GetScoreGoalForCurrentLevel())
        {
            ShowVictoryPanel();
        }
    }

    private void ShowUpgradePanel()
    {
        if (upgradeManager != null)
        {
            upgradeManager.ShowUpgradePanel(); // Delegar la lógica al UpgradeManager
        }
    }

    private void CheckSpecialAttackAvailability()
    {
        // Calcular cuántos múltiplos de 1000 se han alcanzado
        int eligibleActivations = playerScore / scoreThreshold;

        // Si se alcanza un nuevo múltiplo y no se ha activado aún
        if (eligibleActivations > timesSpecialActivated)
        {
            isSpecialAttackAvailable = true;
            timesSpecialActivated = eligibleActivations; // Actualizar el contador
            Debug.Log("Ataque especial desbloqueado!");
        }
    }

    public void UseSpecialAttack()
    {
        if (isSpecialAttackAvailable)
        {
            isSpecialAttackAvailable = false; // Marcar como usado
            Debug.Log("Ataque especial usado.");
        }
        else
        {
            Debug.Log("El ataque especial no está disponible.");
        }
    }

    public void TakeDamage(int damage)
    {
        playerLife -= damage;
        Debug.Log("Vida restante: " + playerLife);
        UpdateLifeUI();

        if (playerLife <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (pauseButton != null)
            pauseButton.SetActive(false);

        Debug.Log("¡Juego Terminado!");
        Time.timeScale = 0f;

        // Sumar puntaje actual al total
        totalScore += playerScore;

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
            if (defeatLevelScoreText != null)
                defeatLevelScoreText.text = "SCORE: " + playerScore;
            if (defeatTotalScoreText != null)
                defeatTotalScoreText.text = "TOTAL SCORE: " + totalScore;

            // Desactivar la música de fondo cuando se muestra el panel de derrota
            GameObject backgroundMusic = GameObject.Find("BackgroundMusic"); // Asume que el objeto se llama "BackgroundMusic"
            if (backgroundMusic != null)
            {
                backgroundMusic.SetActive(false);
                Debug.Log("Música de fondo desactivada.");
            }
        }

        UpdateRetryButton();
    }

    public void ResetAllGameData()
    {
        totalScore = 0;
        playerScore = 0;
        lastUpgradeScore = 0;
        playerLife = initialLife;
        currentRetries = 0;
        Time.timeScale = 1f;
        isSpecialAttackAvailable = false;

        UpdateHUD();
    }


    public void Play()
    {
        ResetAllGameData();
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void BackToMenu()
    {
        Debug.Log("Volviendo al menú principal...");

        // Desuscribirse de eventos
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Restablecer la escala de tiempo
        Time.timeScale = 1f;

        // Cancelar invocaciones pendientes
        CancelInvoke();
        StopAllCoroutines();

        // Reiniciar variables del GameManager
        ResetAllGameData();

        // Mover el GameManager a la escena activa (por si es necesario)
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        // Opción: Desactivar el GameManager en lugar de destruirlo
        gameObject.SetActive(false);

        // Cargar la escena del menú principal
        SceneManager.LoadScene("MainMenu");
    }


    // Método para ir al main screen
    public void Back()
    {
        SceneManager.LoadScene("MainScreen");
    }

    public void RestartGame()
    {
        Debug.Log("Intentando reiniciar el juego...");

        // Reactivar el GameManager si está inactivo
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("GameManager está inactivo. Reactivándolo.");
            gameObject.SetActive(true);
        }

        if (currentRetries >= maxRetries)
        {
            Debug.LogWarning("Máximo de reintentos alcanzado. No se puede reiniciar.");
            return;
        }

        currentRetries++;
        Debug.Log("Reintento número: " + currentRetries);

        remainingPortals = 0;
        totalPortals = 0;
        OnPortalsChanged?.Invoke(0, 0);
        UpdateRetryButton();
        playerLife = initialLife;
        playerScore = 0;
        lastUpgradeScore = 0;
        lastUpgradeScore = 0;
        UpdateHUD();

        // Asegúrate de que el tiempo esté en escala normal
        Time.timeScale = 1f;

        Time.timeScale = 1f;

        // Cierra el DefeatPanel si está activo
        if (defeatPanel != null && defeatPanel.activeSelf)
        {
            defeatPanel.SetActive(false);
            Debug.Log("DefeatPanel cerrado.");

            // Reactivar la música de fondo cuando se cierra el panel de derrota (al reiniciar)
            GameObject backgroundMusic = GameObject.Find("BackgroundMusic"); // Asume que el objeto se llama "BackgroundMusic"
            if (backgroundMusic != null)
            {
                backgroundMusic.SetActive(true);
                Debug.Log("Música de fondo reactivada.");
            }
        }

        // Inicia la corrutina para recargar la escena
        StartCoroutine(ReloadSceneCoroutine());
    }

    private IEnumerator ReloadSceneCoroutine()
    {
        Debug.Log("Iniciando corrutina para recargar la escena...");

        // Simula un retraso para reproducir el sonido, si es necesario
        yield return new WaitForSecondsRealtime(0.15f);

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void UpdateRetryButton()
    {
        if (retryButton != null)
        {
            if (currentRetries >= maxRetries)
            {
                retryButton.interactable = false; // Desactiva el botón
                var colors = retryButton.colors;
                colors.normalColor = Color.gray; // Cambia el color del botón a gris
                retryButton.colors = colors;
            }
            else
            {
                retryButton.interactable = true; // Activa el botón
                var colors = retryButton.colors;
                colors.normalColor = Color.white; // Cambia el color del botón a blanco
                retryButton.colors = colors;
            }
        }
    }



    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Detiene la ejecución del juego en el editor, simulando que sale del juego.
#else
        Application.Quit(); // Cierra la aplicación en compilaciones cuando exportemos el juego.
#endif
    }

    public void UpdateHUD()
    {
        UpdateScoreUI();
        UpdateLifeUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE " + playerScore;
    }

    private void UpdateLifeUI()
    {
        if (lifeText != null)
            lifeText.text = playerLife + "%";
        if (skillsLifeText != null)
            skillsLifeText.text = playerLife + "%";
    }

    private int GetScoreGoalForCurrentLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        foreach (var goal in levelGoals)
        {
            if (goal.sceneIndex == index)
                return goal.requiredScore;
        }
        return int.MaxValue;
    }

    public void ShowVictoryPanel()
    {
        if (pauseButton != null)
            pauseButton.SetActive(false);

        Time.timeScale = 0f;

        // Sumar el puntaje actual al puntaje total
        totalScore += playerScore;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            if (victoryLevelScoreText != null)
                victoryLevelScoreText.text = "SCORE: " + playerScore;

            if (victoryTotalScoreText != null)
                victoryTotalScoreText.text = "TOTAL SCORE: " + totalScore;

            GameObject backgroundMusic = GameObject.Find("BackgroundMusic");
            if (backgroundMusic != null)
            {
                backgroundMusic.SetActive(false);
                Debug.Log("Música de fondo desactivada.");
            }
        }
        else
        {
            Debug.LogWarning("VictoryPanel no asignado.");
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex == totalScenes - 1)
        {
            PlayerPrefs.SetInt("LastLevelCompleted", 1);
            PlayerPrefs.Save();
            Debug.Log("¡Último nivel completado! Progreso guardado.");

            SceneManager.sceneLoaded += UnlockLevelButtonsOnMenu;
        }
    }
    private void UnlockLevelButtonsOnMenu(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Button[] levelButtons = GameObject.FindObjectsOfType<Button>();

            foreach (Button btn in levelButtons)
            {
                if (btn.gameObject.CompareTag("LevelButton"))
                    btn.interactable = true;
            }

            Debug.Log("Botones de niveles desbloqueados directamente desde GameManager.");

            // Se desuscribe para no ejecutarlo en futuras cargas de escena
            SceneManager.sceneLoaded -= UnlockLevelButtonsOnMenu;
        }
    }
    public void ShowVictoryPanelDelayed(float delaySeconds)
    {
        StartCoroutine(VictoryPanelCoroutine(delaySeconds));
    }

    private IEnumerator VictoryPanelCoroutine(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        ShowVictoryPanel();
    }

    public bool IsVictoryPanelActive()
    {
        return victoryPanel != null && victoryPanel.activeSelf;
    }

    public bool IsDefeatPanelActive()
    {
        return defeatPanel != null && defeatPanel.activeSelf;
    }

    public void RegisterPortal()
    {
    }

    public void PortalDestroyed()
    {
        remainingPortals--;
        OnPortalsChanged?.Invoke(remainingPortals, totalPortals);

        if (remainingPortals <= 0)
        {
            CheckVictoryConditionAfterEnemyDeath();
        }
    }


    private GameObject FindInactiveObjectByTag(string tag)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.CompareTag(tag) && t.gameObject.scene.isLoaded)
                return t.gameObject;
        }
        return null;
    }

    public void ShowRemainingEnemiesMessage()
    {
        if (remainingEnemiesGroup == null) return;
        remainingEnemiesMessage.SetActive(true);
        remainingEnemiesGroup.alpha = 1f;

        // Cambia el texto (suponiendo que tu TMP está como hijo)
        TextMeshProUGUI tmp = remainingEnemiesMessage.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = "DEFEAT THE REMAINING ENEMIES!";

        StartCoroutine(FadeOutMessage(2f, 1f));
    }

    private IEnumerator FadeOutMessage(float visibleTime, float fadeTime)
    {
        yield return new WaitForSeconds(visibleTime);
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            remainingEnemiesGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }
        remainingEnemiesGroup.alpha = 0f;
        remainingEnemiesMessage.SetActive(false);
    }

    public int GetAliveEnemyCount()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int bossCount = GameObject.FindGameObjectsWithTag("Boss").Length;
        return enemyCount + bossCount;
    }

    public void CheckVictoryConditionAfterEnemyDeath()
    {
        StartCoroutine(CheckVictoryWithDelay());
    }

    private IEnumerator CheckVictoryWithDelay()
    {
        // Espera 1 frame
        yield return null;

        if (remainingPortals <= 0 && GetAliveEnemyCount() == 0)
        {
            ShowVictoryPanelDelayed(0.5f);
        }
        else if (remainingPortals <= 0 && GetAliveEnemyCount() > 0 && !hasShownRemainingEnemiesMessage)
        {
            ShowRemainingEnemiesMessage();
            hasShownRemainingEnemiesMessage = true;
        }
    }


}