using UnityEngine;

public class UpgradeDataBlackHole : MonoBehaviour
{
    public float extraLifeSpan = 0f;      // Duración adicional del agujero negro
    public float extraSuctionForce = 0f; // Fuerza de succión adicional

    // Aplicar mejora de duración al prefab
    public void ApplyLifeSpanUpgrade(BlackHoleSkill blackHolePrefab)
    {
        if (blackHolePrefab == null) return;

        blackHolePrefab.SetLifeSpan(blackHolePrefab.lifeSpan + extraLifeSpan);
        Debug.Log($"Black Hole Lifespan en el prefab actualizado a: {blackHolePrefab.lifeSpan}");
    }

    // Aplicar mejora de fuerza de succión al prefab
    public void ApplySuctionForceUpgrade(BlackHoleSkill blackHolePrefab)
    {
        if (blackHolePrefab == null) return;

        blackHolePrefab.SetSuctionForce(blackHolePrefab.suctionForce + extraSuctionForce);
        Debug.Log($"Black Hole Suction Force en el prefab actualizado a: {blackHolePrefab.suctionForce}");
    }
}