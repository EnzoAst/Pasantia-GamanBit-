using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float detectionRadius = 5f;
    public Transform firePoint;
    public Animator animator;
    private AudioSource audioSource;
    public AudioClip bulletSound;

    // Duración de vida de la torreta
    public float duration = 10f;

    private float nextFireTime;

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
        Debug.Log($"Turret Duration actualizado a: {duration}s");
    }

    void Start()
    {
        // Aplicar upgrades acumulados
        SetDuration(duration + UpgradeDataManager.Instance.GetTurretUpgrade("duration"));

        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, duration);
    }

    void Update()
    {
        if (SkillPanelController.IsSkillPanelOpen)
            return;

        GameObject target = FindNearestEnemy();

        if (target != null && Time.time >= nextFireTime)
        {
            FireAt(target);
            nextFireTime = Time.time + 1f / fireRate;
        }

        // Asegurarse de siempre actualizar la animación a Idle si no hay enemigo cerca
        if (target == null && animator != null)
        {
            animator.SetInteger("Direction", -1);
        }
    }

    GameObject FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float minDist = Mathf.Infinity;
        GameObject nearest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") || (hit.CompareTag("Boss")))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.gameObject;
                }
            }
        }

        return nearest;
    }

    void FireAt(GameObject target)
    {
        // Dirección normalizada hacia el enemigo
        Vector2 dir = (target.transform.position - firePoint.position).normalized;

        // Verificar si se necesita voltear la torreta
        Flip(dir);

        // Ángulo de rotación para que la bala apunte hacia la dirección
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Instanciar la bala rotada correctamente
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));

        // Asignar velocidad en esa dirección
        bullet.GetComponent<Rigidbody2D>().velocity = dir * 10f;

        // 👉 SONIDO de disparo
        if (bulletSound != null && audioSource != null)
            audioSource.PlayOneShot(bulletSound);

        // Animación direccional de la torreta
        PlayDirectionAnimation(dir);
    }

    void PlayDirectionAnimation(Vector2 dir)
    {
        if (animator == null) return;

        // Si mira a la izquierda, trata la dirección como si fuera a la derecha (para la animación)
        Vector2 animDir = dir;
        if (dir.x < 0) animDir.x = -animDir.x;

        int directionValue = 0;
        if (animDir.x > 0)
        {
            if (animDir.y > 0.5f)
                directionValue = 1; // Arriba Derecha
            else if (animDir.y < -0.5f)
                directionValue = 4; // Abajo Derecha
            else
                directionValue = 0; // Derecha
        }
        animator.SetInteger("Direction", directionValue);
    }

    void Flip(Vector2 dir)
    {
        // Volteá solo si el sprite debe mirar a la izquierda
        bool lookLeft = dir.x < 0;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (lookLeft ? -1 : 1);
        transform.localScale = scale;
    }
}