using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    public Slider slider;


    public void SetMaxHealth(int enhealth)
    {
        slider.maxValue = enhealth;
        slider.value = enhealth;
    }

    public void SetHealth(int enhealth)
    {
        slider.value = enhealth;
    }
}
