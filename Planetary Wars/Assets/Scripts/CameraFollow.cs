using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referencia al objeto del jugador
    public float smoothSpeed = 0.125f; // Velocidad de seguimiento
    public Vector3 offset; // Desplazamiento de la c�mara respecto al jugador
    public int pixelsPerUnit = 32; // Valor com�n: 32, 64 o 100 seg�n los sprites

    void LateUpdate()
    {
        if (player == null) return;

        // Posici�n deseada con offset
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Redondear posici�n para evitar tearing (solo x e y, mantener z)
        float snapX = Mathf.Round(smoothedPosition.x * pixelsPerUnit) / pixelsPerUnit;
        float snapY = Mathf.Round(smoothedPosition.y * pixelsPerUnit) / pixelsPerUnit;

        transform.position = new Vector3(snapX, snapY, smoothedPosition.z);
    }
}
