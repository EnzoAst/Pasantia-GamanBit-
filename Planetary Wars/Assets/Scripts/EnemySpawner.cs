using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo
    public Transform spawnPoint; // Punto de generaci�n de enemigos
    public float spawnInterval = 2f; // Intervalo entre cada aparici�n de enemigos (por tiempo)
    public float spawnDuration = 10f; // Duraci�n total del spawn de enemigos
    public string playerTag = "Player"; // Tag del jugador

    public bool usePointsToSpawn = false; // Activar o desactivar spawn por puntos
    public int pointsPerSpawn = 10; // Puntos necesarios para generar un enemigo

    private Transform player; // Referencia al jugador
    private int lastSpawnScore = 0; // Puntaje registrado del �ltimo spawn
    private Coroutine spawnCoroutine; // Referencia a la corrutina activa

    void Start()
    {
        // Desactiva este script si est� en el men� principal
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
            Debug.LogWarning("No se encontr� un objeto con el tag 'Player'.");
        }

        // Iniciar el spawn de enemigos seg�n la configuraci�n
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
            Debug.LogWarning("SpawnPoint o EnemyPrefab no est� asignado.");
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
            Debug.LogWarning("SpawnPoint o EnemyPrefab no est� asignado.");
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
                lastSpawnScore = GameManager.instance.playerScore; // Actualizar el puntaje del �ltimo spawn
            }

            yield return null; // Esperar al siguiente frame
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoint == null || enemyPrefab == null)
        {
            Debug.LogWarning("SpawnPoint o EnemyPrefab no est� asignado. No se puede generar enemigos.");
            return;
        }

        // Crear enemigo
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyBehavior behavior = enemy.GetComponent<EnemyBehavior>();

        // Configurar los par�metros del enemigo
        if (behavior != null && player != null)
        {
            behavior.Initialize(player);
        }
    }
}