using UnityEngine;

public class AllySpawner : MonoBehaviour
{
    public GameObject allyPrefab; // Prefab del aliado
    public Transform player; // Referencia al jugador
    public float spawnRadius = 2f; // Distancia a la que aparecerán los aliados
    public string itemTag = "AllyItem"; // Tag del objeto que activa el spawn

    private int allyCount = 0; // Contador de aliados

    void Start()
    {
        // Encontrar al jugador por su tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag 'Player'.");
        }
    }

    // Método llamado cuando se recoge un ítem
    public void SpawnAllies(int numberOfAllies)
    {
        for (int i = 0; i < numberOfAllies; i++)
        {
            // Generar una posición aleatoria alrededor del jugador
            Vector2 spawnPosition = player.position + (Vector3)(Random.insideUnitCircle.normalized * spawnRadius);

            // Crear aliado en la posición generada
            GameObject ally = Instantiate(allyPrefab, spawnPosition, Quaternion.identity);

            // Ignorar colisión entre el aliado y el jugador
            Collider2D allyCollider = ally.GetComponent<Collider2D>();
            Collider2D playerCollider = player.GetComponent<Collider2D>();

            if (allyCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(allyCollider, playerCollider);
            }

            allyCount++; // Aumentar el contador de aliados
        }
    }

    // Detectar colisiones con los ítems
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(itemTag))
        {
            SpawnAllies(1);

            // Destruir el ítem recolectado
            Destroy(collision.gameObject);
        }
    }
}
