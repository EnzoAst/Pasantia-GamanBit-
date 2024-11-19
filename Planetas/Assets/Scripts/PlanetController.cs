using UnityEngine;

public class PlanetController : MonoBehaviour

{

    public float speed;
    public bool invert; 

    void Start()
    {
        
    }
    void Update()
    {
        if (!invert)
            transform.Rotate(new Vector3(0, 0, speed) * Time.deltaTime);
        else
            transform.Rotate(new Vector3(0, 0, speed * (-1)) * Time.deltaTime);

    }
}
