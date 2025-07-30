using UnityEngine;
using TMPro;

public class SkillPanelController : MonoBehaviour
{
    [SerializeField] private GameObject skillPanel;

    [Header("Referencias a otros paneles críticos")]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject buttonPause;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject upgradePanel;

    [Header("Player Attributes")]
    [SerializeField] private TextMeshProUGUI txtLongShot;
    [SerializeField] private TextMeshProUGUI txtPlayerSpeed;

    [Header("Bullet")]
    [SerializeField] private TextMeshProUGUI txtBulletSpeed;
    [SerializeField] private TextMeshProUGUI txtBulletDamage;

    [Header("Shock Wave")]
    [SerializeField] private TextMeshProUGUI txtWaveRadius;
    [SerializeField] private TextMeshProUGUI txtWavePower;
    [SerializeField] private TextMeshProUGUI txtWaveCooldown;

    [Header("Black Hole")]
    [SerializeField] private TextMeshProUGUI txtBlackHoleCooldown;
    [SerializeField] private TextMeshProUGUI txtBlackHoleSuction;
    [SerializeField] private TextMeshProUGUI txtBlackHoleLifespan;

    [Header("Turret")]
    [SerializeField] private TextMeshProUGUI txtTurretDuration;

    [Header("Prefabs/Base Instances")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private WaveAbility wavePrefab;
    [SerializeField] private BlackHoleSkill blackHolePrefab;
    [SerializeField] private Turret turretPrefab;

    private bool isVisible = false;

    public static bool IsSkillPanelOpen { get; private set; } = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!AnyCriticalPanelActive())
            {
                TogglePanel();
            }
        }
        else if (isVisible && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
    }

    bool AnyCriticalPanelActive()
    {
        return (storyPanel != null && storyPanel.activeInHierarchy)
            || (panelPause != null && panelPause.activeInHierarchy)
            || (buttonPause != null && panelPause.activeInHierarchy)
            || (helpPanel != null && helpPanel.activeInHierarchy)
            || (defeatPanel != null && defeatPanel.activeInHierarchy)
            || (victoryPanel != null && victoryPanel.activeInHierarchy)
            || (upgradePanel != null && upgradePanel.activeInHierarchy);
    }

    void TogglePanel()
    {
        isVisible = !isVisible;
        skillPanel.SetActive(isVisible);
        IsSkillPanelOpen = isVisible;

        // Oculta el botón de pausa cuando está el skill panel, lo muestra cuando no
        if (buttonPause != null)
            buttonPause.SetActive(!isVisible);

        if (isVisible)
        {
            Time.timeScale = 0;
            PauseAllAudioExceptBackground();
            UpdateSkillTexts();
        }
        else
        {
            Time.timeScale = 1;
            ResumeAllAudio();

            // Si el panel de pausa o cualquier otro panel que pause el juego está activo, se de pausa el juego
            if ((panelPause != null && panelPause.activeInHierarchy) ||
                (defeatPanel != null && defeatPanel.activeInHierarchy) ||
                (victoryPanel != null && victoryPanel.activeInHierarchy) ||
                (upgradePanel != null && upgradePanel.activeInHierarchy))
            {
                Time.timeScale = 0;
            }
        }
    }

    void PauseAllAudioExceptBackground()
    {
        foreach (AudioSource src in FindObjectsOfType<AudioSource>())
        {
            if (src.CompareTag("Music")) continue;
            src.Pause();
        }
    }

    void ResumeAllAudio()
    {
        foreach (AudioSource src in FindObjectsOfType<AudioSource>())
        {
            if (src.CompareTag("Music")) continue;
            src.UnPause();
        }
    }

    void UpdateSkillTexts()
    {
        // PLAYER
        var player = FindObjectOfType<PlayerController>();
        float playerSpeed = player != null ? player.playerSpeed : 0f;
        txtPlayerSpeed.text = playerSpeed.ToString("0.0");

        float detectionRadius = player != null ? player.enemyDetectionRadius : 0f;
        txtLongShot.text = detectionRadius.ToString("0");

        // BULLET
        float baseBulletSpeed = bulletPrefab != null ? bulletPrefab.bulletSpeed : 0f;
        float bulletSpeedUpgrade = UpgradeDataManager.Instance.GetBulletUpgrade("speed");
        txtBulletSpeed.text = (baseBulletSpeed + bulletSpeedUpgrade).ToString("0");

        int baseBulletDamage = bulletPrefab != null ? bulletPrefab.damage : 0;
        int bulletDamageUpgrade = (int)UpgradeDataManager.Instance.GetBulletUpgrade("damage");
        txtBulletDamage.text = (baseBulletDamage + bulletDamageUpgrade).ToString("0");

        // SHOCK WAVE
        float baseWaveRadius = wavePrefab != null ? wavePrefab.waveRadius : 0f;
        float waveRadiusUpgrade = UpgradeDataManager.Instance.GetWaveUpgrade("radius");
        txtWaveRadius.text = (baseWaveRadius + waveRadiusUpgrade).ToString("0");

        float baseWaveForce = wavePrefab != null ? wavePrefab.waveForce : 0f;
        float waveForceUpgrade = UpgradeDataManager.Instance.GetWaveUpgrade("force");
        txtWavePower.text = (baseWaveForce + waveForceUpgrade).ToString("0");

        float waveCooldown = player != null ? player.waveCooldown : 0f;
        txtWaveCooldown.text = waveCooldown.ToString("0") + "s";

        // BLACK HOLE
        float blackHoleCooldown = player != null ? player.cooldownTime : 0f;
        txtBlackHoleCooldown.text = blackHoleCooldown.ToString("0") + "s";

        float baseBlackHoleSuction = blackHolePrefab != null ? blackHolePrefab.suctionForce : 0f;
        float blackHoleSuctionUpgrade = UpgradeDataManager.Instance.GetBlackHoleUpgrade("suction");
        txtBlackHoleSuction.text = (baseBlackHoleSuction + blackHoleSuctionUpgrade).ToString("0");

        float baseBlackHoleLifespan = blackHolePrefab != null ? blackHolePrefab.lifeSpan : 0f;
        float blackHoleLifespanUpgrade = UpgradeDataManager.Instance.GetBlackHoleUpgrade("lifespan");
        txtBlackHoleLifespan.text = (baseBlackHoleLifespan + blackHoleLifespanUpgrade).ToString("0") + "s";

        // TURRET
        float baseTurretDuration = turretPrefab != null ? turretPrefab.duration : 0f;
        float turretDurationUpgrade = UpgradeDataManager.Instance.GetTurretUpgrade("duration");
        txtTurretDuration.text = (baseTurretDuration + turretDurationUpgrade).ToString("0") + "s";
    }
}