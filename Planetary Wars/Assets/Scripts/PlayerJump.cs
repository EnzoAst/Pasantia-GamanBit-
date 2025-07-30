using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 10f; // Fuerza del salto
    private Rigidbody2D rb;
    private bool enSuelo; // Saber si está tocando el suelo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Rock"))
        {
            enSuelo = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Rock"))
        {
            enSuelo = false;
        }
    }
}
