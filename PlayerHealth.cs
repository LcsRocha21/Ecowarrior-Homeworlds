using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int currentHealth, maxHealth, DamageAmount;
    public Healthbar healthbar;

    public float immortalTime;
    private float immortalCounter;

    public GameObject immortalEffect;

    public GameObject gameoverScreen;

    private Animator anim;
    public PlayerController playerController;

    public GameObject dieEffect;

    public int healthpickupAmount = 1;
    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(currentHealth);
        immortalEffect.SetActive(false);
        gameoverScreen.SetActive(false);
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (immortalCounter > 0)
        {
            immortalCounter -= Time.deltaTime;
            if (immortalCounter <= 0)
            {
                immortalEffect.SetActive(false);
            }
        }
    }

    public void DealDamage()
    {
        if (immortalCounter <= 0)
        {
            currentHealth -= DamageAmount;
            AudioManager.instance.PlaySfx(1);
            healthbar.SetHealth(currentHealth);

            //Se a vida do Player chegar a zero, ele sumirá da cena
            if (currentHealth <= 0)
            {
                StartCoroutine(DieCo());
            }

            else
            {
                immortalCounter = immortalTime;
                immortalEffect.SetActive(true);
            }
        }
    }

    IEnumerator DieCo()
    {
        playerController.enabled = false;
        Instantiate(dieEffect, transform.position, transform.rotation);

        anim.SetTrigger("Die");
        yield return new WaitForSeconds(3f);
        gameoverScreen.SetActive(true);
        AudioManager.instance.StopBackgroundMusic();
        Time.timeScale = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Health")
        {
            AudioManager.instance.PlaySfx(0);
            currentHealth += healthpickupAmount;
            healthbar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            Destroy(other.gameObject);
        }
    }
}
