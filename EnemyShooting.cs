using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePosition;
    public float timeBetweenshots = 1f;
    private float timer;
    void Start()
    {

    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenshots)
        {
            Shoot();
            timer = 0f;

        }
    }


    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
    }
}
