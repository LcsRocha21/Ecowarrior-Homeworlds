using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    // Referência para o ReturnToMainMenuVoiceController
    private ReturnToMainMenuVoiceController returnVoiceController;

    void Start()
    {
        // Obtém referência do ReturnToMainMenuVoiceController
        returnVoiceController = GetComponent<ReturnToMainMenuVoiceController>();
        if (returnVoiceController == null)
        {
            returnVoiceController = FindObjectOfType<ReturnToMainMenuVoiceController>();
        }
        
        if (returnVoiceController == null)
        {
            Debug.LogWarning("ReturnToMainMenu: ReturnToMainMenuVoiceController não encontrado. Comandos de voz não estarão disponíveis.");
        } else {
            Debug.Log("ReturnToMainMenu: ReturnToMainMenuVoiceController encontrado com sucesso!");
        }
    }

    public void ReturnToMainMenuScene()
    {
        Debug.Log("ReturnToMainMenu: ReturnToMainMenuScene() chamado - Carregando cena MainMenu...");
        SceneManager.LoadScene("MainMenu");
    }
    
    // Métodos públicos para controle externo dos comandos de voz
    public void EnableVoiceCommands()
    {
        if (returnVoiceController != null)
        {
            returnVoiceController.EnableVoiceCommands();
            Debug.Log("ReturnToMainMenu: Solicitando habilitação de comandos de voz para retornar ao menu principal.");
        }
    }
    
    public void DisableVoiceCommands()
    {
        if (returnVoiceController != null)
        {
            returnVoiceController.DisableVoiceCommands();
            Debug.Log("ReturnToMainMenu: Solicitando desabilitação de comandos de voz para retornar ao menu principal.");
        }
    }
}
