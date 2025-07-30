using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo
    public Transform spawnPoint; // Punto de generación de enemigos
    public float spawnInterval = 2f; // Intervalo entre cada aparición de enemigos (por tiempo)
    public float spawnDuration = 10f; // Duración total del spawn de enemigos
    public string playerTag = "Player"; // Tag del jugador

    public bool usePointsToSpawn = false; // Activar o desactivar spawn por puntos
    public int pointsPerSpawn = 10; // Puntos necesarios para generar un enemigo

    private Transform player; // Referencia al jugador
    private int lastSpawnScore = 0; // Puntaje registrado del último spawn
    private Coroutine spawnCoroutine; // Referencia a la corrutina activa

    void Start()
    {
        // Desactiva este script si está en el menú principal
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            this.enabled = false;
            return;
        }

        // Encontrar al jugador por su tag
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag 'Player'.");
        }

        // Iniciar el spawn de enemigos según la configuración
        if (usePointsToSpawn)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemiesByPoints());
        }
        else
        {
            spawnCoroutine = StartCoroutine(SpawnEnemiesByTime());
        }
    }

    void OnDisable()
    {
        // Detener cualquier corrutina activa al desactivar el script
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    IEnumerator SpawnEnemiesByTime()
    {
        if (spawnPoint == null || enemyPrefab == null)
        {
            Debug.LogWarning("SpawnPoint o EnemyPrefab no está asignado.");
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < spawnDuration)
        {
            SpawnEnemy(); // Generar el enemigo
            elapsedTime += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnEnemiesByPoints()
    {
        if (spawnPoint == null || enemyPrefab == null)
        {
            Debug.LogWarning("SpawnPoint o EnemyPrefab no está asignado.");
            yield break;
        }

        while (true)
        {
            if (GameManager.instance == null)
            {
                Debug.LogWarning("GameManager no encontrado. Deteniendo SpawnEnemiesByPoints.");
                yield break;
            }

            if (GameManager.instance.playerScore >= lastSpawnScore + pointsPerSpawn)
            {
                SpawnEnemy(); // Generar el enemigo
                lastSpawnScore = GameManager.instance.playerScore; // Actualizar el puntaje del último spawn
            }

            yield return null; // Esperar al siguiente frame
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoint == null || enemyPrefab == null)
        {
            Debug.LogWarning("SpawnPoint o EnemyPrefab no está asignado. No se puede generar enemigos.");
            return;
        }

        // Crear enemigo
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyBehavior behavior = enemy.GetComponent<EnemyBehavior>();

        // Configurar los parámetros del enemigo
        if (behavior != null && player != null)
        {
            behavior.Initialize(player);
        }
    }
}