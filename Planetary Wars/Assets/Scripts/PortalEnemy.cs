using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PortalEnemy : MonoBehaviour
{
    public int health = 50;
    private int maxHealth;
    public GameObject dropPrefab;
    public GameObject closedPortalPrefab;
    public GameObject healthBarPrefab;
    private Image healthFill;
    private GameObject healthBarInstance;
    private CanvasGroup healthBarCanvasGroup;

    private bool isDead = false;
    private Coroutine hideBarCoroutine;

    void Start()
    {
        maxHealth = health;

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPortal();
        }
        healthBarInstance = Instantiate(healthBarPrefab, transform);
        healthBarInstance.transform.localPosition = new Vector3(0, -1.5f, 0);

        healthFill = healthBarInstance.transform.Find("Background/Fill").GetComponent<Image>();
        if (healthFill != null)
            healthFill.fillAmount = 1f;

        // Obtené el CanvasGroup del prefab de barra de vida
        healthBarCanvasGroup = healthBarInstance.GetComponent<CanvasGroup>();
        if (healthBarCanvasGroup == null)
        {
            healthBarCanvasGroup = healthBarInstance.AddComponent<CanvasGroup>();
        }

        // Ocultá la barra de vida al inicio
        healthBarCanvasGroup.alpha = 0f;
        healthBarInstance.SetActive(true); // Debe estar activo para el fade
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
                collision.gameObject.SetActive(false);
            }
        }
        else if (collision.CompareTag("Missile"))
        {
            HomingMissile missile = collision.GetComponent<HomingMissile>();
            if (missile != null)
            {
                TakeDamage(missile.damage);
                collision.gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        health = Mathf.Max(health, 0);

        // Muestro la barra con fade al recibir daño
        ShowHealthBarFade();

        if (healthFill != null)
        {
            healthFill.fillAmount = (float)health / maxHealth;
        }

        // Reinicio el temporizador para ocultar la barra con fade
        if (hideBarCoroutine != null)
        {
            StopCoroutine(hideBarCoroutine);
        }
        hideBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(2f, 0.7f)); // 2 seg delay, 0.7s fade

        if (health <= 0)
        {
            Die();
        }
    }

    // Corrutina que oculta la barra con fade tras un delay
    private IEnumerator HideHealthBarAfterDelay(float delay, float fadeDuration)
    {
        yield return new WaitForSeconds(delay);
        yield return FadeHealthBar(healthBarCanvasGroup.alpha, 0f, fadeDuration);
    }

    // Hago aparecer la barra con fade rápido
    private void ShowHealthBarFade()
    {
        if (hideBarCoroutine != null)
        {
            StopCoroutine(hideBarCoroutine);
            hideBarCoroutine = null;
        }
        StartCoroutine(FadeHealthBar(healthBarCanvasGroup.alpha, 1f, 0.2f)); // Fade in rápido
    }

    // Corrutina de fade
    private IEnumerator FadeHealthBar(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthBarCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        healthBarCanvasGroup.alpha = to;
    }

    public void Shrink(float shrinkSpeed)
    {
        // No hacer nada, el portal no se achica
    }

    private void Die()
    {
        isDead = true;
        DropItem();
        if (closedPortalPrefab != null)
        {
            GameObject closedPortal = Instantiate(closedPortalPrefab, transform.position, Quaternion.identity);
            Destroy(closedPortal, 0.5f);
        }
        if (GameManager.instance != null)
        {
            GameManager.instance.PortalDestroyed();
        }
        Destroy(gameObject);
    }

    private void DropItem()
    {
        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
    }
}