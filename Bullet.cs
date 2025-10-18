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
        if (other.gameObject.CompareTag("Enemy"))
        {

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage();
            }
            else if (other.GetComponent<Enemy4>() is Enemy4 enemyV4)
            {
                enemyV4.TakeDamage();
            }
            else if (other.GetComponent<Boss>() is Boss boss)
            {
                boss.TakeDamage();
            }
        }
        
        Instantiate(hitEffectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}


