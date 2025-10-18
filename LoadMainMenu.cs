using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    private LoadMainMenuVoiceController loadMainMenuVoiceController;

    void Start()
    {
        loadMainMenuVoiceController = GetComponent<LoadMainMenuVoiceController>();
        if (loadMainMenuVoiceController == null)
        {
            loadMainMenuVoiceController = FindObjectOfType<LoadMainMenuVoiceController>();
        }
        
        if (loadMainMenuVoiceController == null)
        {
            Debug.LogWarning("LoadMainMenu: LoadMainMenuVoiceController não encontrado. Comandos de voz não estarão disponíveis para carregar o menu principal.");
        } else {
            Debug.Log("LoadMainMenu: LoadMainMenuVoiceController encontrado com sucesso!");
        }
    }

    public void LoadScene()
    {
        Debug.Log("LoadMainMenu: LoadScene() chamado - Carregando cena MainMenu...");
        SceneManager.LoadScene("MainMenu");
    }
    
    public void EnableVoiceCommands()
    {
        if (loadMainMenuVoiceController != null)
        {
            loadMainMenuVoiceController.EnableVoiceCommands();
            Debug.Log("LoadMainMenu: Solicitando habilitação de comandos de voz para carregar menu principal.");
        }
    }
    
    public void DisableVoiceCommands()
    {
        if (loadMainMenuVoiceController != null)
        {
            loadMainMenuVoiceController.DisableVoiceCommands();
            Debug.Log("LoadMainMenu: Solicitando desabilitação de comandos de voz para carregar menu principal.");
        }
    }
}

