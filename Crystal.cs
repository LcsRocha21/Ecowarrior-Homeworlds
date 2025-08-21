using UnityEngine;

public class Crystal : MonoBehaviour
{
    public float rotationSpeed = 10f;

    void Start()
    {
        
    }


    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
