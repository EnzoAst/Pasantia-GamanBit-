using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateMissileAbility : MonoBehaviour
{
    public GameObject missilePrefab; // Prefab del misil
    public int missileCount = 10; // Cantidad de misiles a lanzar
    public float spawnRadius = 1f; // Radio alrededor del astronauta para lanzar los misiles
    public float delayBetweenMissiles = 0.1f; // Tiempo entre cada misil
    public float freezeDuration = 5f; // Duración en segundos del congelamiento de los enemigos

    private List<Transform> enemies;

    void Start()
    {
        enemies = new List<Transform>();
    }

    public void Activate()
    {
        if (GameManager.instance != null && GameManager.instance.isSpecialAttackAvailable)
        {
            GameManager.instance.UseSpecialAttack(); // Marcar el ataque como usado

            // Activar el efecto de oscurecimiento
            if (ScreenDarkener.instance != null)
            {
                ScreenDarkener.instance.FadeIn(0.5f); // Oscurecer en 0.5 segundos
            }

            FreezeEnemies();
            StartCoroutine(SpawnMissiles());

            // Desactivar el efecto de oscurecimiento después del tiempo de congelamiento
            StartCoroutine(DeactivateDarkening());
        }
        else
        {
            Debug.Log("El ataque especial no está disponible.");
        }
    }

    private void FreezeEnemies()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bossObjects = GameObject.FindGameObjectsWithTag("Boss");

        foreach (GameObject enemy in enemyObjects)
        {
            StartCoroutine(FreezeEnemy(enemy));
        }

        foreach (GameObject boss in bossObjects)
        {
            StartCoroutine(FreezeEnemy(boss));
        }
    }

    private IEnumerator FreezeEnemy(GameObject enemy)
    {
        if (enemy == null)
            yield break;

        MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            // Evita deshabilitar EnemySpawner y PortalEnemy
            if (script.GetType() == typeof(EnemySpawner) || script.GetType() == typeof(PortalEnemy))
                continue;
            script.enabled = false;
        }

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(freezeDuration);

        if (enemy == null)
            yield break;

        foreach (MonoBehaviour script in scripts)
        {
            if (script.GetType() == typeof(EnemySpawner) || script.GetType() == typeof(PortalEnemy))
                continue;
            script.enabled = true;
        }
    }

    private IEnumerator DeactivateDarkening()
    {
        yield return new WaitForSeconds(freezeDuration);

        if (ScreenDarkener.instance != null)
        {
            ScreenDarkener.instance.FadeOut(0.5f);
        }
    }

    private IEnumerator SpawnMissiles()
    {
        enemies.Clear();
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bossObjects = GameObject.FindGameObjectsWithTag("Boss");

        foreach (GameObject enemy in enemyObjects)
        {
            enemies.Add(enemy.transform);
        }
        foreach (GameObject boss in bossObjects)
        {
            enemies.Add(boss.transform);
        }

        for (int i = 0; i < missileCount; i++)
        {
            Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
            GameObject missile = Instantiate(missilePrefab, spawnPosition, Quaternion.identity);

            if (enemies.Count > 0)
            {
                Transform target = enemies[Random.Range(0, enemies.Count)];
                missile.GetComponent<HomingMissile>().SetTarget(target);
            }

            yield return new WaitForSeconds(delayBetweenMissiles);
        }

        Debug.Log("¡Ataque especial completado!");
    }
}
