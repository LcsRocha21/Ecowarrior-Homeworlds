using UnityEngine;
using UnityEngine.UI;

public class Pickups : MonoBehaviour
{
    public int gems = 0;
    public GameObject pickupEffect;

    public Text gemText;

    void Start()
    {
        gemText.text = "CRISTAIS: 0";
    }


    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gem")
        {
            AudioManager.instance.PlaySfx(0);
            Instantiate(pickupEffect, transform.position, transform.rotation);
            gems++;
            gemText.text = "CRISTAIS: " + gems;
            Destroy(other.gameObject);
        }
    }
}
