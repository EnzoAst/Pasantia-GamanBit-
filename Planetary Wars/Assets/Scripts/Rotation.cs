using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Transform[] planets; // Array de planetas
    public float speed = 1f; // Velocidad de movimiento

    private Vector3[] targetPositions; // Posiciones objetivo para los planetas

    void Start()
    {
        // Almacenar las posiciones originales de los planetas
        targetPositions = new Vector3[planets.Length];
        for (int i = 0; i < planets.Length; i++)
        {
            targetPositions[i] = planets[i].position;
        }
    }

    void Update()
    {
        // Mover cada planeta hacia su posición objetivo
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].position = Vector3.MoveTowards(planets[i].position, targetPositions[i], speed * Time.deltaTime);
        }

        // Comprobar si todos los planetas han llegado a sus posiciones objetivo
        if (AllPlanetsReachedTargets())
        {
            // Intercambiar las posiciones objetivo hacia la derecha
            ShiftTargetsRight();
        }
    }

    private bool AllPlanetsReachedTargets()
    {
        // Verificar si todos los planetas están en su posición objetivo
        for (int i = 0; i < planets.Length; i++)
        {
            if (Vector3.Distance(planets[i].position, targetPositions[i]) > 0.01f)
            {
                return false;
            }
        }
        return true;
    }

    private void ShiftTargetsRight()
    {
        // Guardar la última posición objetivo
        Vector3 lastTarget = targetPositions[targetPositions.Length - 1];

        // Mover cada posición objetivo un lugar a la derecha
        for (int i = targetPositions.Length - 1; i > 0; i--)
        {
            targetPositions[i] = targetPositions[i - 1];
        }

        // Asignar la última posición objetivo al primer planeta
        targetPositions[0] = lastTarget;
    }
}