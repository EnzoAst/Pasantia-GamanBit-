using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public GameObject collectiblePrefab; // Prefab del collectible
    public Transform[] spawnPoints; // Puntos de aparición

    private void Start()
    {
        GenerarCollectibles();
    }

    public void GenerarCollectibles()
    {
        // Eliminar todos los collectibles existentes
        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("Collectible"))
        {
            Destroy(collectible);
        }

        // Generar nuevos collectibles en los puntos de aparición
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(collectiblePrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
