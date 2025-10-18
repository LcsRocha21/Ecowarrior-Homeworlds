using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseScreen;
    public bool isPaused;

    public static Pause instance;

    private PauseVoiceController pauseVoiceController;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pauseVoiceController = GetComponent<PauseVoiceController>();
        if (pauseVoiceController == null)
        {
            pauseVoiceController = FindObjectOfType<PauseVoiceController>();
        }
        
        if (pauseVoiceController == null)
        {
            Debug.LogWarning("Pause: PauseVoiceController não encontrado. Comandos de voz não estarão disponíveis no menu de pausa.");
        } else {
            Debug.Log("Pause: PauseVoiceController encontrado com sucesso!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        if (isPaused)
        {
            Debug.Log("Pause: PauseUnpause() chamado - Despausando jogo...");
            isPaused = false;
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Debug.Log("Pause: PauseUnpause() chamado - Pausando jogo...");
            isPaused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void MenuButton()
    {
        Debug.Log("Pause: MenuButton() chamado - Voltando ao menu principal...");
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void RestartButton()
    {
        Debug.Log("Pause: RestartButton() chamado - Reiniciando cena...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void EnableVoiceCommands()
    {
        if (pauseVoiceController != null)
        {
            pauseVoiceController.EnableVoiceCommands();
            Debug.Log("Pause: Solicitando habilitação de comandos de voz no menu de pausa.");
        }
    }
    
    public void DisableVoiceCommands()
    {
        if (pauseVoiceController != null)
        {
            pauseVoiceController.DisableVoiceCommands();
            Debug.Log("Pause: Solicitando desabilitação de comandos de voz no menu de pausa.");
        }
    }
}

