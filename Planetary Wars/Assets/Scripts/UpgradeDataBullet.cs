using UnityEngine;

public class UpgradeDataBullet : MonoBehaviour
{
    public float extraSpeed = 0f;
    public int extraDamage = 0;

public void ApplyUpgradesToPrefab(Bullet bulletPrefab)
    {
        if (bulletPrefab == null) return;

        bulletPrefab.SetBulletSpeed(bulletPrefab.bulletSpeed + extraSpeed);
        bulletPrefab.SetDamage(bulletPrefab.damage + extraDamage);
        Debug.Log($"Velocidad del proyectil en el prefab actualizada a {bulletPrefab.bulletSpeed}");
        Debug.Log($"Daño del proyectil en el prefab actualizada a {bulletPrefab.damage}");
    }
}