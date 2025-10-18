using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform pointA, pointB;
    public int speed;
    private Vector3 currentTarget;

    public int currentHealth, maxHealth, damageAmount;

    private Animator anim;

    public GameObject explosion;

    public EnemyHealthbar enemyHealthbar;

    void Start()
    {
        currentHealth = maxHealth;
        enemyHealthbar.SetMaxHealth(currentHealth);

        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (transform.position == pointA.position)
        {
            currentTarget = pointB.position;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        else if (transform.position == pointB.position)
        {
            currentTarget = pointA.position;
            transform.localScale = new Vector3(0.5f, 0.5f, -0.5f);
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        currentHealth -= damageAmount;
        AudioManager.instance.PlaySfx(1);
        enemyHealthbar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        AudioManager.instance.PlaySfx(2);
        anim.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 2f);

        Debug.Log("Enemy is dead");
    }
}
