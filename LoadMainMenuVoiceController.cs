using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class LoadMainMenuVoiceController : MonoBehaviour
{
    [Header("Voice Recognition Settings")]
    public float confidenceThreshold = 0.5f;
    public bool enableVoiceCommands = true;
    
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    
    private LoadMainMenu loadMainMenuController;
    
    void Start()
    {
        loadMainMenuController = GetComponent<LoadMainMenu>();
        if (loadMainMenuController == null)
        {
            loadMainMenuController = FindObjectOfType<LoadMainMenu>();
        }
        
        if (loadMainMenuController == null)
        {
            Debug.LogError("LoadMainMenuVoiceController: LoadMainMenu não encontrado!");
            return;
        }
        else
        {
            Debug.Log("LoadMainMenuVoiceController: LoadMainMenu encontrado com sucesso!");
        }
        
        SetupVoiceCommands();
        
        if (enableVoiceCommands)
        {
            StartVoiceRecognition();
        }
    }
    
    void SetupVoiceCommands()
    {
        Debug.Log("LoadMainMenuVoiceController: Entrando em SetupVoiceCommands().");

        keywords.Clear();

        // Comandos para carregar o menu principal
        keywords.Add("menu principal", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("voltar ao menu", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("ir para o menu", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("menu", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("sair", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("voltar", () => OnVoiceCommand("loadMainMenu"));
        
        // Comandos em inglês
        keywords.Add("main menu", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("go to menu", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("back to menu", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("exit", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("quit", () => OnVoiceCommand("loadMainMenu"));
        keywords.Add("back", () => OnVoiceCommand("loadMainMenu"));

        Debug.Log($"LoadMainMenuVoiceController: {keywords.Count} palavras-chave adicionadas.");
    }
    
    void StartVoiceRecognition()
    {
        Debug.Log($"LoadMainMenuVoiceController: Tentando iniciar reconhecimento com {keywords.Count} palavras-chave.");

        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            Debug.LogWarning("LoadMainMenuVoiceController: Reconhecimento de voz já está rodando. Ignorando Start().");
            return;
        }

        if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Start();
            Debug.Log("LoadMainMenuVoiceController: Reconhecimento de voz reiniciado.");
            return;
        }

        if (keywordRecognizer == null)
        {
            if (keywords.Count == 0)
            {
                Debug.LogError("LoadMainMenuVoiceController: Não há palavras-chave para iniciar o reconhecimento. Certifique-se de que SetupVoiceCommands() foi chamado e adicionou palavras-chave.");
                return;
            }
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();
            Debug.Log("LoadMainMenuVoiceController: Reconhecimento de voz iniciado pela primeira vez.");
        }
    }
    
    void StopVoiceRecognition()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            Debug.Log("LoadMainMenuVoiceController: Reconhecimento de voz parado");
        }
    }
    
    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (!enableVoiceCommands) return;
        
        float recognizedConfidenceValue = 0f;
        switch (args.confidence)
        {
            case ConfidenceLevel.High:
                recognizedConfidenceValue = 0.8f;
                break;
            case ConfidenceLevel.Medium:
                recognizedConfidenceValue = 0.6f;
                break;
            case ConfidenceLevel.Low:
                recognizedConfidenceValue = 0.3f;
                break;
            case ConfidenceLevel.Rejected:
                recognizedConfidenceValue = 0.0f;
                break;
        }

        // Verifica o nível de confiança
        if (recognizedConfidenceValue >= confidenceThreshold)
        {
            Debug.Log($"LoadMainMenuVoiceController: Comando de voz reconhecido: \'{args.text}\' (Confiança: {args.confidence})");
            
            if (keywords.ContainsKey(args.text))
            {
                keywords[args.text].Invoke();
            }
        }
        else
        {
            Debug.Log($"LoadMainMenuVoiceController: Comando de voz ignorado por baixa confiança: \'{args.text}\' (Confiança: {args.confidence})");
        }
    }
    
    void OnVoiceCommand(string command)
    {
        Debug.Log($"LoadMainMenuVoiceController: OnVoiceCommand chamado para: {command}");
        switch (command)
        {
            case "loadMainMenu":
                if (loadMainMenuController != null)
                {
                    Debug.Log("LoadMainMenuVoiceController: Executando LoadMainMenu.LoadScene()");
                    loadMainMenuController.LoadScene();
                }
                else
                {
                    Debug.LogError("LoadMainMenuVoiceController: LoadMainMenu não referenciado para 'loadMainMenu' command.");
                }
                break;
        }
    }
    

    public void EnableVoiceCommands()
    {
        enableVoiceCommands = true;
        Debug.Log("LoadMainMenuVoiceController: Comandos de voz habilitados.");
        StartVoiceRecognition();
    }
    
    public void DisableVoiceCommands()
    {
        enableVoiceCommands = false;
        Debug.Log("LoadMainMenuVoiceController: Comandos de voz desabilitados.");
        StopVoiceRecognition();
        Debug.Log("LoadMainMenuVoiceController: Comandos de voz ativos resetados.");
    }
    
    public void SetConfidenceThreshold(float threshold)
    {
        confidenceThreshold = Mathf.Clamp01(threshold);
        Debug.Log($"LoadMainMenuVoiceController: Confidence Threshold ajustado para: {confidenceThreshold}");
    }
    
    void OnDestroy()
    {
        StopVoiceRecognition();
        
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Dispose();
            Debug.Log("LoadMainMenuVoiceController: KeywordRecognizer disposed.");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            StopVoiceRecognition();
            Debug.Log("LoadMainMenuVoiceController: Aplicação pausada, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("LoadMainMenuVoiceController: Aplicação despausada, reconhecimento reiniciada.");
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            StopVoiceRecognition();
            Debug.Log("LoadMainMenuVoiceController: Aplicação perdeu foco, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("LoadMainMenuVoiceController: Aplicação ganhou foco, reconhecimento reiniciada.");
        }
    }
}
