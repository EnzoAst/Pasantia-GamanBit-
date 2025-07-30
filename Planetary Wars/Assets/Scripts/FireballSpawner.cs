using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab; // Prefab de la bola de fuego
    public float spawnInterval = 2f; // Tiempo entre cada spawn
    public float spawnRangeX = 5f; // Rango en el eje X para el spawn
    public float spawnHeight = 20f; // Altura fija en el eje Y donde nacen las bolas de fuego
    public float fireballSpeed = 5f; // Velocidad de ca�da de las bolas de fuego

    public float minY = -10f; // L�mite inferior del rango de destrucci�n
    public float maxY = -30f; // L�mite superior del rango de destrucci�n

    private void Start()
    {
        // Iniciar la generaci�n de bolas de fuego
        InvokeRepeating(nameof(SpawnFireball), 0f, spawnInterval);
    }

    private void SpawnFireball()
    {
        // Generar una posici�n aleatoria en el eje X dentro del rango
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0f);

        // Instanciar la bola de fuego en la posici�n generada
        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

        // Calcular un punto final aleatorio en el eje Y para esta bola de fuego
        float randomEndY = Random.Range(minY, maxY);

        // Agregar el componente FireballMover a la bola de fuego
        FireballMover mover = fireball.GetComponent<FireballMover>();
        if (mover != null)
        {
            mover.SetSpeed(fireballSpeed);
            mover.SetEndY(randomEndY); // Asignar un punto final aleatorio
        }
    }
}