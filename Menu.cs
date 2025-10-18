using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string levelName;
    public string creditsName;
    public string optionsName;

    private MenuVoiceController menuVoiceController;

    void Start()
    {
        menuVoiceController = GetComponent<MenuVoiceController>();
        if (menuVoiceController == null)
        {
            menuVoiceController = FindObjectOfType<MenuVoiceController>();
        }
        
        if (menuVoiceController == null)
        {
            Debug.LogWarning("Menu: MenuVoiceController não encontrado. Comandos de voz não estarão disponíveis no menu.");
        } else {
            Debug.Log("Menu: MenuVoiceController encontrado com sucesso!");
        }
    }

    public void StartGame()
    {
        Debug.Log("Menu: StartGame() chamado - Carregando cena do jogo...");
        SceneManager.LoadScene(levelName);
    }

    public void CreditsScene()
    {
        Debug.Log("Menu: CreditsScene() chamado - Carregando cena de créditos...");
        SceneManager.LoadScene(creditsName);
    }

    public void OptionsScene()
    {
        Debug.Log("Menu: OptionsScene() chamado - Carregando cena de opções...");
        SceneManager.LoadScene(optionsName);
    }

    public void leaveGame()
    {
        Debug.Log("Menu: leaveGame() chamado - Encerrando aplicação...");
        Application.Quit();
        Debug.Log("Leaving the game");
    }

    public void EnableVoiceCommands()
    {
        if (menuVoiceController != null)
        {
            menuVoiceController.EnableVoiceCommands();
            Debug.Log("Menu: Solicitando habilitação de comandos de voz no menu.");
        }
    }
    
    public void DisableVoiceCommands()
    {
        if (menuVoiceController != null)
        {
            menuVoiceController.DisableVoiceCommands();
            Debug.Log("Menu: Solicitando desabilitação de comandos de voz no menu.");
        }
    }
}
