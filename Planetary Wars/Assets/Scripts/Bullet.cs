using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public int damage = 25;
    private Rigidbody2D rigidbody2d;

    public GameObject explosionPrefab; // Prefab de la explosión

    public void SetBulletSpeed(float newSpeed)
    {
        bulletSpeed = newSpeed;
        Debug.Log($"Bullet speed actualizado a: {bulletSpeed}");
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
        Debug.Log($"Bullet damage actualizado a: {damage}");
    }

    void Start()
    {
        // Aplicar upgrades acumulados
        SetBulletSpeed(bulletSpeed + UpgradeDataManager.Instance.GetBulletUpgrade("speed"));
        SetDamage(damage + Mathf.RoundToInt(UpgradeDataManager.Instance.GetBulletUpgrade("damage")));

        rigidbody2d = GetComponent<Rigidbody2D>();

        // Buscar el collider de la bala
        Collider2D bulletCollider = GetComponent<Collider2D>();
        if (bulletCollider == null) return;

        // Buscar todos los colliders del fuego en la escena y desactivar colisión con ellos
        GameObject[] fireObjects = GameObject.FindGameObjectsWithTag("Fire");
        foreach (GameObject fire in fireObjects)
        {
            Collider2D fireCollider = fire.GetComponent<Collider2D>();
            if (fireCollider != null)
            {
                Physics2D.IgnoreCollision(bulletCollider, fireCollider, true);
            }
        }
    }

    private void OnBecameVisible()
    {
        rigidbody2d.velocity = transform.right * bulletSpeed;
    }

    private void OnBecameInvisible()
    {
        Invoke("Destroy", 0.25f);
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si la bala toca el fuego, simplemente lo ignora y sigue su camino
        if (collision.CompareTag("Fire")) return;

        // Verifica si la bala impacta en un enemigo o en el entorno
        if (collision.CompareTag("Enemy") || collision.CompareTag("Environment") || collision.CompareTag("Boss"))
        {
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 0.7f); // La explosión desaparece después de 0.7s
            }

            gameObject.SetActive(false); // Desactiva la bala al impactar
        }
    }
}