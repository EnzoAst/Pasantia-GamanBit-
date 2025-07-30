using UnityEngine;

public class TumbleweedMovement : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float direction = 1f;
    [SerializeField] private float rotationSpeed = 100f; // Velocidad de rotación

    [Header("Rebote")]
    [SerializeField] private float bounceForce = 2f; // Fuerza del rebote

    [Header("Sombra")]
    [SerializeField] private Transform shadow;
    [SerializeField] private float shadowOffsetY = -0.5f; // Distancia entre la bola y la sombra

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        // Movimiento horizontal
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);

        // Rotación para simular el rodar
        transform.Rotate(Vector3.forward * -rotationSpeed * direction * Time.deltaTime);

        // Mantener la sombra debajo de la bola sin rotarla
        if (shadow != null)
        {
            shadow.position = new Vector3(transform.position.x, transform.position.y + shadowOffsetY, transform.position.z);
            shadow.rotation = Quaternion.identity; // Evita que la sombra rote
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Al tocar un objeto con las etiquetas Limit, Player o Enemy
        if (collision.gameObject.CompareTag("Limit"))
        {
            // Invertir dirección al tocar los límites
            direction *= -1;
        }
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            // Rebote en diagonal con una fuerza ajustable
            rb.velocity = new Vector2(-direction * speed, bounceForce);
        }
    }
}
