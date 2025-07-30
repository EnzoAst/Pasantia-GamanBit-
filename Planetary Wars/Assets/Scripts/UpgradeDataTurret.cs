using UnityEngine;

public class UpgradeDataTurret : MonoBehaviour
{
    public float extraDuration = 0f;

    public void ApplyUpgradesToPrefab(Turret turretPrefab)
    {
        if (turretPrefab == null) return;

        turretPrefab.SetDuration(turretPrefab.duration + extraDuration);
        Debug.Log($"Duraci�n de la torreta en el prefab actualizada a {turretPrefab.duration}");
    }
}