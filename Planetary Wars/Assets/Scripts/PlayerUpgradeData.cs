using UnityEngine;

public class PlayerUpgradeData : MonoBehaviour
{
    public int extraLife = 0;  // Aumento de vida máxima y actual
    public float extraSpeed = 0f;  // Mejora de velocidad del jugador
    public float extraBlackHoleDuration = 0f;  // Reducción del cooldown del agujero negro
    public float extraWaveCooldownReduction = 0f;  // Reducción del cooldown de la onda expansiva
    public float extraDetectionRadius = 0f;  // Aumento del radio de detección de enemigos

    public void ApplyUpgradesTo(PlayerController player, GameManager gameManager)
    {
        if (player == null || gameManager == null) return;

        // Aplicar mejoras de movimiento y cooldowns
        player.playerSpeed += extraSpeed;
        player.cooldownTime -= extraBlackHoleDuration;
        player.waveCooldown -= extraWaveCooldownReduction;
        player.enemyDetectionRadius += extraDetectionRadius;

        // Aplicar mejora de vida
        gameManager.initialLife += extraLife;
        gameManager.playerLife += extraLife;

        // Limitar vida actual a la nueva vida máxima
        if (gameManager.playerLife > gameManager.initialLife)
            gameManager.playerLife = gameManager.initialLife;
    }

    // Método para obtener la descripción de la mejora
    public string GetDescription()
    {
        string description = "";

        if (extraLife > 0)
            description += $"Extra Life: +{extraLife} ";
        if (extraSpeed > 0)
            description += $"Speed: +{extraSpeed} ";
        if (extraBlackHoleDuration > 0)
            description += $"Black Hole Cooldown Reduction: -{extraBlackHoleDuration}s ";
        if (extraWaveCooldownReduction > 0)
            description += $"Wave Cooldown Reduction: -{extraWaveCooldownReduction}s ";
        if (extraDetectionRadius > 0)
            description += $"Detection Radius: +{extraDetectionRadius}";

        return description.Trim();
    }

    public void ResetCooldownAndDetectionUpgrades()
    {
        extraBlackHoleDuration = 0f;
        extraWaveCooldownReduction = 0f;
        extraDetectionRadius = 0f;
    }
}