using UnityEngine;
using TMPro;
public class Collectible : MonoBehaviour
{
    public int points = 10;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;

    public float attractionRadius = 2f;
    public float attractionSpeed = 5f;

    [Header("Audio")]
    public AudioClip collectSound;
    private AudioSource audioSource;

    [Header("UI")]
    public GameObject floatingTextPrefab;  // El prefab del texto flotante

    private Vector3 startPosition;
    private Transform player;

    void Start()
    {
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        Vector3 floatPosition = new Vector3(startPosition.x, newY, startPosition.z);

        if (player != null && Vector3.Distance(transform.position, player.position) < attractionRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, attractionSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = floatPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.AddScore(points);

            if (floatingTextPrefab != null)
            {
                Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
                if (canvas != null)
                {
                    // Convertir la posición del mundo a pantalla
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

                    screenPos.x += Random.Range(-70f, 70f); // más cerca en horizontal
                    screenPos.y += Random.Range(-70f, 70f);  // menos altura

                    Transform textLayer = canvas.transform.Find("FloatingTextLayer");
                    GameObject text = Instantiate(floatingTextPrefab, screenPos, Quaternion.identity, textLayer);

                    text.transform.SetAsLastSibling(); // Esto lo pone al frente de todos los elementos UI
                    Debug.Log("Texto flotante instanciado dentro del Canvas");

                    TMP_Text textMeshPro = text.GetComponent<TMP_Text>();
                    if (textMeshPro != null)
                    {
                        textMeshPro.text = "+" + points.ToString();
                    }
                    else
                    {
                        Debug.LogError("No TMP_Text component found in the floating text prefab.");
                    }

                    Debug.Log("Texto flotante instanciado con offset en pantalla.");
                }
                else
                {
                    Debug.LogError("No se encontró un Canvas en la escena.");
                }
            }

            if (collectSound != null && audioSource != null)
                audioSource.PlayOneShot(collectSound);

            Destroy(gameObject, 0.3f);
        }
    }
}