using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public GameObject hitEffectPrefab;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }


    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Instantiate(hitEffectPrefab, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Instantiate(hitEffectPrefab, transform.position, transform.rotation);
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage();
            }
        }
        Destroy(gameObject);
    }

}
