using System.Collections.Generic;
using UnityEngine;

// Sistema centralizado para almacenar mejoras aplicadas
public class UpgradeDataManager : MonoBehaviour
{
    public static UpgradeDataManager Instance;

    // Diccionarios para almacenar mejoras
    private Dictionary<string, float> waveUpgrades = new Dictionary<string, float>();
    private Dictionary<string, float> turretUpgrades = new Dictionary<string, float>();
    private Dictionary<string, float> blackHoleUpgrades = new Dictionary<string, float>();
    private Dictionary<string, float> bulletUpgrades = new Dictionary<string, float>();
    private Dictionary<string, float> playerUpgrades = new Dictionary<string, float>(); // NUEVO: Player upgrades persistentes

    private void Awake()
    {
        // Crear Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Métodos para Wave
    public void AddWaveUpgrade(string key, float value)
    {
        if (!waveUpgrades.ContainsKey(key))
            waveUpgrades[key] = 0;

        waveUpgrades[key] += value;

        Debug.Log($"[UpgradeDataManager] Mejora agregada a Wave: {key} = {waveUpgrades[key]}");
    }

    public float GetWaveUpgrade(string key)
    {
        float value = waveUpgrades.ContainsKey(key) ? waveUpgrades[key] : 0;
        Debug.Log($"[UpgradeDataManager] Mejora obtenida para Wave: {key} = {value}");
        return value;
    }

    // Métodos para Turret
    public void AddTurretUpgrade(string key, float value)
    {
        if (!turretUpgrades.ContainsKey(key))
            turretUpgrades[key] = 0;

        turretUpgrades[key] += value;
    }

    public float GetTurretUpgrade(string key)
    {
        return turretUpgrades.ContainsKey(key) ? turretUpgrades[key] : 0;
    }

    // Métodos para Black Hole
    public void AddBlackHoleUpgrade(string key, float value)
    {
        if (!blackHoleUpgrades.ContainsKey(key))
            blackHoleUpgrades[key] = 0;

        blackHoleUpgrades[key] += value;
    }

    public float GetBlackHoleUpgrade(string key)
    {
        return blackHoleUpgrades.ContainsKey(key) ? blackHoleUpgrades[key] : 0;
    }

    // Métodos para Bullet
    public void AddBulletUpgrade(string key, float value)
    {
        if (!bulletUpgrades.ContainsKey(key))
            bulletUpgrades[key] = 0;

        bulletUpgrades[key] += value;
    }

    public float GetBulletUpgrade(string key)
    {
        return bulletUpgrades.ContainsKey(key) ? bulletUpgrades[key] : 0;
    }

    // Métodos para Player upgrades (EXCEPTO velocidad, que es temporal por escena)
    public void AddPlayerUpgrade(string key, float value)
    {
        if (!playerUpgrades.ContainsKey(key))
            playerUpgrades[key] = 0;

        playerUpgrades[key] += value;

        Debug.Log($"[UpgradeDataManager] Mejora agregada a Player: {key} = {playerUpgrades[key]}");
    }

    public float GetPlayerUpgrade(string key)
    {
        float value = playerUpgrades.ContainsKey(key) ? playerUpgrades[key] : 0;
        Debug.Log($"[UpgradeDataManager] Mejora obtenida para Player: {key} = {value}");
        return value;
    }

    public void ResetAllUpgrades()
    {
        waveUpgrades.Clear();
        turretUpgrades.Clear();
        blackHoleUpgrades.Clear();
        bulletUpgrades.Clear();
        playerUpgrades.Clear();
    }
}