using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class History1 : MonoBehaviour
{
    public GameObject historyScreen;
    public string levelName;

    void Start()
    {
        StartCoroutine(HistoryRun());
    }


    void Update()
    {

    }

    IEnumerator HistoryRun()
    {
        yield return new WaitForSeconds(0.5f);
        historyScreen.SetActive(true);
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene(levelName);
    }
}
