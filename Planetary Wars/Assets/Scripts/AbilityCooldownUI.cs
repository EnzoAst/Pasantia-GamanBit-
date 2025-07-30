using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    public Image blackHoleFill;
    public Image waveFill;
    public Image turretFill;
    public GameObject missileLockObject;
    public PlayerController player;
    public GameObject fKeyOverlay;
    public GameObject eKeyOverlay;
    public GameObject tKeyOverlay;
    public GameObject mKeyOverlay;


    void Update()
    {
        // Black Hole
        float bhRemaining = player.nextAvailableTime - Time.time;
        blackHoleFill.fillAmount = Mathf.Clamp01(bhRemaining / player.cooldownTime);
        fKeyOverlay.SetActive(bhRemaining > 0);

        // Wave
        float waveRemaining = (player.lastWaveTime + player.waveCooldown) - Time.time;
        waveFill.fillAmount = Mathf.Clamp01(waveRemaining / player.waveCooldown);
        eKeyOverlay.SetActive(waveRemaining > 0);

        // Turret
        float turretRemaining = player.nextTurretTime - Time.time;
        turretFill.fillAmount = Mathf.Clamp01(turretRemaining / player.turretCooldown);
        tKeyOverlay.SetActive(turretRemaining > 0);

        // Missile
        if (GameManager.instance != null)
        {
            bool isMissileAvailable = GameManager.instance.isSpecialAttackAvailable;
            missileLockObject.SetActive(!isMissileAvailable);
            mKeyOverlay.SetActive(!isMissileAvailable);
        }
    }

}