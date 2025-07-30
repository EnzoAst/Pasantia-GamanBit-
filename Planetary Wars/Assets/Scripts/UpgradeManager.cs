using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrades Config")]
    public List<MonoBehaviour> allUpgrades; // Lista de todas las habilidades

    [Header("UI Config")]
    [SerializeField] private List<Button> upgradeButtons;
    [SerializeField] private GameObject upgradePanel;

    [Header("Sprites Config")]
    [SerializeField] private List<Sprite> upgradeSprites;

    public object UpgradePanel { get; internal set; }

    private void Awake()
    {
        // Asignar referencias automáticamente si faltan
        if (upgradePanel == null || upgradeButtons.Any(b => b == null))
        {
            AssignInactiveUIReferences();
        }

        if (upgradePanel != null)
            upgradePanel.SetActive(false); // Asegurar que el panel esté desactivado al inicio
        if (FindObjectsOfType<UpgradeManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignInactiveUIReferences();
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        if (scene.name == "MainMenu")
        {
            ResetUpgrades();
        }
    }

    private void AssignInactiveUIReferences()
    {
        // Buscar y asignar el panel inactivo por nombre
        if (upgradePanel == null)
        {
            upgradePanel = FindInactiveObjectByName("UpgradePanel");
        }

        // Buscar y asignar los botones inactivos por nombre
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (upgradeButtons[i] == null)
            {
                var buttonObj = FindInactiveObjectByName($"UpgradeButton{i + 1}");
                if (buttonObj != null)
                {
                    upgradeButtons[i] = buttonObj.GetComponent<Button>();
                }
            }
        }
    }

    // Método para buscar objetos inactivos por nombre en la escena
    private GameObject FindInactiveObjectByName(string name)
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            GameObject found = FindInChildrenIncludingInactive(obj.transform, name);
            if (found != null)
                return found;
        }
        return null;
    }

    private GameObject FindInChildrenIncludingInactive(Transform parent, string name)
    {
        if (parent.name == name)
            return parent.gameObject;
        foreach (Transform child in parent)
        {
            GameObject found = FindInChildrenIncludingInactive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    public void ShowUpgradePanel()
    {
        Time.timeScale = 0; // Pausar el juego
        PauseMenu.escapeEnabled = false; // Desactivar tecla esc, para evitar problemas
        GenerateRandomUpgrades();

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true); // Mostrar el panel
        }
    }

    private void GenerateRandomUpgrades()
    {
        // Crear una lista temporal para mejoras individuales
        var individualUpgrades = new List<(string description, System.Action apply, int spriteIndex)>();

        foreach (var upgrade in allUpgrades)
        {
            // Mejoras de Wave
            if (upgrade is UpgradeDataWave wave)
            {
                if (wave.extraWaveForce > 0)
                {
                    individualUpgrades.Add((
                        $"SHOCKWAVE POWER +{wave.extraWaveForce}",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddWaveUpgrade("force", wave.extraWaveForce);
                            Debug.Log($"[UpgradeManager] Mejora de fuerza de onda aplicada: +{wave.extraWaveForce}");
                        },
                        0 // Índice del sprite para Shockwave Power
                    ));
                }

                if (wave.extraWaveRadius > 0)
                {
                    individualUpgrades.Add((
                        $"SHOCKWAVE RADIUS +{wave.extraWaveRadius}",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddWaveUpgrade("radius", wave.extraWaveRadius);
                            Debug.Log($"[UpgradeManager] Mejora de radio de onda aplicada: +{wave.extraWaveRadius}");
                        },
                        1 // Índice del sprite para Shockwave Radius
                    ));
                }
            }

            // Mejoras de Bullet
            else if (upgrade is UpgradeDataBullet bullet)
            {
                if (bullet.extraSpeed > 0)
                    individualUpgrades.Add((
                        $"BULLET SPEED +{bullet.extraSpeed}",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddBulletUpgrade("speed", bullet.extraSpeed);
                            Debug.Log("Mejora de velocidad de proyectil aplicada.");
                        },
                        2 // Índice del sprite para Bullet Speed
                    ));

                if (bullet.extraDamage > 0)
                    individualUpgrades.Add((
                        $"BULLET DAMAGE +{bullet.extraDamage}",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddBulletUpgrade("damage", bullet.extraDamage);
                            Debug.Log("Mejora de daño de proyectil aplicada.");
                        },
                        3 // Índice del sprite para Bullet Damage
                    ));
            }

            // Mejoras de Black Hole
            else if (upgrade is UpgradeDataBlackHole blackHole)
            {
                if (blackHole.extraLifeSpan > 0)
                    individualUpgrades.Add((
                        $"BLACK HOLE LIFESPAN +{blackHole.extraLifeSpan}s",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddBlackHoleUpgrade("lifespan", blackHole.extraLifeSpan);
                            Debug.Log("Mejora de duración de agujero negro aplicada.");
                        },
                        4 // Índice del sprite para Black Hole Lifespan
                    ));

                if (blackHole.extraSuctionForce > 0)
                    individualUpgrades.Add((
                        $"BLACK HOLE SUCTION FORCE +{blackHole.extraSuctionForce}",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddBlackHoleUpgrade("suction", blackHole.extraSuctionForce);
                            Debug.Log("Mejora de fuerza de succión aplicada.");
                        },
                        5 // Índice del sprite para Black Hole Suction Force
                    ));
            }

            // Mejoras de Turret
            else if (upgrade is UpgradeDataTurret turret)
            {
                if (turret.extraDuration > 0)
                    individualUpgrades.Add((
                        $"TURRET DURATION +{turret.extraDuration}s",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddTurretUpgrade("duration", turret.extraDuration);
                            Debug.Log("Mejora de duración de torreta aplicada.");
                        },
                        6 // Índice del sprite para Turret Duration
                    ));
            }

            // Mejoras de Player SEPARADAS (incluyendo cooldowns negativos)
            else if (upgrade is PlayerUpgradeData playerUpgrade)
            {
                // Vida extra
                if (playerUpgrade.extraLife != 0)
                {
                    individualUpgrades.Add((
                        $"HEALTH UPGRADE +{playerUpgrade.extraLife}",
                        () =>
                        {
                            var player = FindObjectOfType<PlayerController>();
                            var gameManager = FindObjectOfType<GameManager>();
                            if (player != null && gameManager != null)
                            {
                                gameManager.initialLife += playerUpgrade.extraLife;
                                gameManager.playerLife += playerUpgrade.extraLife;
                                if (gameManager.playerLife > gameManager.initialLife)
                                    gameManager.playerLife = gameManager.initialLife;
                                Debug.Log("[UpgradeManager] Mejora de vida aplicada: +" + playerUpgrade.extraLife);
                                gameManager.UpdateHUD();
                            }
                        },
                        7 // Índice del sprite para Health Upgrade
                    ));
                }
                // Velocidad extra (NO persiste, se suma solo al player actual y se reinicia por escena)
                if (playerUpgrade.extraSpeed != 0)
                {
                    individualUpgrades.Add((
                        $"PLAYER SPEED +{playerUpgrade.extraSpeed}",
                        () =>
                        {
                            var player = FindObjectOfType<PlayerController>();
                            if (player != null)
                            {
                                player.playerSpeed += playerUpgrade.extraSpeed;
                                Debug.Log("[UpgradeManager] Mejora de velocidad aplicada: +" + playerUpgrade.extraSpeed);
                            }
                            // NO almacenar en UpgradeDataManager para velocidad
                        },
                        8 // Índice del sprite para Player Speed
                    ));
                }
                // Reducción cooldown Black Hole (puede ser negativo)
                if (playerUpgrade.extraBlackHoleDuration != 0)
                {
                    individualUpgrades.Add((
                        $"BLACK HOLE COOLDOWN REDUCTION {playerUpgrade.extraBlackHoleDuration}s",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddPlayerUpgrade("blackHoleCooldown", playerUpgrade.extraBlackHoleDuration);
                            var player = FindObjectOfType<PlayerController>();
                            if (player != null)
                            {
                                player.cooldownTime += playerUpgrade.extraBlackHoleDuration;
                                Debug.Log("[UpgradeManager] Reducción de cooldown de Black Hole aplicada: " + playerUpgrade.extraBlackHoleDuration + "s");
                            }
                        },
                        9 // Índice del sprite para Black Hole Cooldown Reduction
                    ));
                }
                // Reducción cooldown Wave (puede ser negativo)
                if (playerUpgrade.extraWaveCooldownReduction != 0)
                {
                    individualUpgrades.Add((
                        $"SHOCKWAVE COOLDOWN REDUCTION {playerUpgrade.extraWaveCooldownReduction}s",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddPlayerUpgrade("waveCooldown", playerUpgrade.extraWaveCooldownReduction);
                            var player = FindObjectOfType<PlayerController>();
                            if (player != null)
                            {
                                player.waveCooldown += playerUpgrade.extraWaveCooldownReduction;
                                Debug.Log("[UpgradeManager] Reducción de cooldown de Wave aplicada: " + playerUpgrade.extraWaveCooldownReduction + "s");
                            }
                        },
                        10 // Índice del sprite para Shockwave Cooldown Reduction
                    ));
                }
                // Radio de detección extra
                if (playerUpgrade.extraDetectionRadius != 0)
                {
                    individualUpgrades.Add((
                        $"LONG SHOT +{playerUpgrade.extraDetectionRadius}",
                        () =>
                        {
                            UpgradeDataManager.Instance.AddPlayerUpgrade("detectionRadius", playerUpgrade.extraDetectionRadius);
                            var player = FindObjectOfType<PlayerController>();
                            if (player != null)
                            {
                                player.enemyDetectionRadius += playerUpgrade.extraDetectionRadius;
                                Debug.Log("[UpgradeManager] Mejora de radio de detección aplicada: +" + playerUpgrade.extraDetectionRadius);
                            }
                        },
                        11 // Índice del sprite para Detection Radius
                    ));
                }
            }
        }

        individualUpgrades = individualUpgrades
            .GroupBy(u => u.description)
            .Select(g => g.First())
            .ToList();

        // Seleccionar 3 mejoras únicas al azar
        var selectedUpgrades = individualUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        // Asignar las mejoras seleccionadas a los botones
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < selectedUpgrades.Count)
            {
                var (description, applyAction, spriteIndex) = selectedUpgrades[i];
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() =>
                {
                    applyAction();
                    CloseUpgradePanel();
                });

                // Actualizar el texto del botón
                var buttonText = upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = description;
                }

                // Asignar el sprite correspondiente y cambiar color si corresponde
                var buttonImage = upgradeButtons[i].GetComponentInChildren<Image>();
                if (buttonImage != null && upgradeSprites != null && spriteIndex >= 0 && spriteIndex < upgradeSprites.Count)
                {
                    buttonImage.sprite = upgradeSprites[spriteIndex];
                    // Cambiar color a rojo para los upgrades de Wave y Bullet (índices 0, 1, 2, 3, 10)
                    if (spriteIndex == 0 || spriteIndex == 1 || spriteIndex == 2 || spriteIndex == 3 || spriteIndex == 10)
                        buttonImage.color = new Color32(199, 45, 45, 255); // #C72D2D
                    else
                        buttonImage.color = Color.white; // Color por defecto para los demás
                }
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void CloseUpgradePanel()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        // Volver a activar Escape al cerrar el panel 
        PauseMenu.escapeEnabled = true;

        if (GameManager.instance != null && GameManager.instance.pauseButton != null)
            GameManager.instance.pauseButton.SetActive(true);

        Time.timeScale = 1; // Reanudar el juego 
    }

    public void ResetPlayerCooldownAndDetectionUpgrades()
    {
        foreach (var upgrade in allUpgrades)
        {
            if (upgrade is PlayerUpgradeData playerUpgrade)
            {
                playerUpgrade.ResetCooldownAndDetectionUpgrades();
            }
        }
    }

    public void ResetUpgrades()
    {
        UpgradeDataManager.Instance.ResetAllUpgrades();
        ResetPlayerCooldownAndDetectionUpgrades();
    }
}