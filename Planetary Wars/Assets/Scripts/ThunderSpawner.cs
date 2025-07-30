using UnityEngine;

public class ThunderSpawner : MonoBehaviour
{
    [Header("Prefab & Timing")]
    public GameObject thunderPrefab;   // Tu GameObject de trueno
    public float spawnInterval = 8f;   // Segundos entre cada trueno

    [Header("Spawn Area")]
    public Vector2 minPosition;        // Esquina inferior izquierda del área
    public Vector2 maxPosition;        // Esquina superior derecha del área

    [Header("Thunder Sounds")]
    public AudioClip[] thunderSounds;
    private AudioSource audioSource;


    private float timer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnThunder();
            timer = 0f;
        }
    }

    void SpawnThunder()
    {
        // Elegir posición aleatoria dentro del rectángulo definido
        float x = Random.Range(minPosition.x, maxPosition.x);
        float y = Random.Range(minPosition.y, maxPosition.y);
        Vector3 pos = new Vector3(x, y, 0f);

        // Instanciar el trueno
        GameObject t = Instantiate(thunderPrefab, pos, Quaternion.identity);
        if (thunderSounds.Length > 0 && audioSource != null)
        {
            int index = Random.Range(0, thunderSounds.Length);
            audioSource.PlayOneShot(thunderSounds[index]);
        }


        // Opcional: destruirlo cuando termine su animación (asumimos 1s de duración)
        Destroy(t, 2f);
    }

    // Para visualizar el área en el Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Vector3 center = new Vector3(
            (minPosition.x + maxPosition.x) * 0.5f,
            (minPosition.y + maxPosition.y) * 0.5f,
            0f
        );
        Vector3 size = new Vector3(
            Mathf.Abs(maxPosition.x - minPosition.x),
            Mathf.Abs(maxPosition.y - minPosition.y),
            0f
        );
        Gizmos.DrawCube(center, size);
    }
}
