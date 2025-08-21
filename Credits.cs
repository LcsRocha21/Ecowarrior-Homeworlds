using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public GameObject creditsScreen;

    void Start()
    {
        StartCoroutine(CreditsRun());
    }


    void Update()
    {

    }

    IEnumerator CreditsRun()
    {
        yield return new WaitForSeconds(0.5f);
        creditsScreen.SetActive(true);
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene(0);
    }
}
