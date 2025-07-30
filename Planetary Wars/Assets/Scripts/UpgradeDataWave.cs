using UnityEngine;

public class UpgradeDataWave : MonoBehaviour
{
    public float extraWaveForce = 0f;  // Incremento en la fuerza de la onda
    public float extraWaveRadius = 0f; // Incremento en el radio de la onda

    // Aplicar aumento de fuerza al prefab
    public void ApplyForceUpgrade(WaveAbility wavePrefab)
    {
        if (wavePrefab == null) return;

        wavePrefab.SetWaveForce(wavePrefab.waveForce + extraWaveForce);
        Debug.Log($"Wave Force en el prefab actualizado a: {wavePrefab.waveForce}");
    }

    // Aplicar aumento de radio al prefab
    public void ApplyRadiusUpgrade(WaveAbility wavePrefab)
    {
        if (wavePrefab == null) return;

        wavePrefab.SetWaveRadius(wavePrefab.waveRadius + extraWaveRadius);
        Debug.Log($"Wave Radius en el prefab actualizado a: {wavePrefab.waveRadius}");
    }
}