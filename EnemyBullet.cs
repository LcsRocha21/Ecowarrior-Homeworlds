using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;
    public GameObject hitEffect;
    void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        Destroy(gameObject, 5f);
    }


    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
            Destroy(gameObject);

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth.DealDamage();
        }
    }
}
