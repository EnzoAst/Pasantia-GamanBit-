using UnityEngine;

public class ShootingStar : MonoBehaviour
{
    public Vector3 startPoint;      // Punto inicial (arriba a la derecha)
    public Vector3 endPoint;        // Punto final (abajo a la izquierda)
    public float speed = 5f;        // Velocidad de movimiento
    private Vector3 direction;

    void Start()
    {
        transform.position = startPoint;
        direction = (endPoint - startPoint).normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Si llega cerca del destino, reiniciamos
        if (Vector3.Distance(transform.position, endPoint) < 0.5f)
        {
            transform.position = startPoint;
        }
    }
}
