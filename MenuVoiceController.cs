using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class MenuVoiceController : MonoBehaviour
{
    [Header("Voice Recognition Settings")]
    public float confidenceThreshold = 0.5f;
    public bool enableVoiceCommands = true;
    
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    
    private Menu menuController;
    
    [HideInInspector] public bool voiceStartGame = false;
    [HideInInspector] public bool voiceCredits = false;
    [HideInInspector] public bool voiceOptions = false;
    [HideInInspector] public bool voiceQuit = false;
    
    void Start()
    {
        menuController = GetComponent<Menu>();
        if (menuController == null)
        {
            menuController = FindObjectOfType<Menu>();
        }
        
        if (menuController == null)
        {
            Debug.LogError("MenuVoiceController: Menu não encontrado!");
            return;
        }
        
        SetupVoiceCommands();
        
        if (enableVoiceCommands)
        {
            StartVoiceRecognition();
        }
    }
    
    void SetupVoiceCommands()
    {
        Debug.Log("MenuVoiceController: Entrando em SetupVoiceCommands().");

        keywords.Clear();

        keywords.Add("iniciar jogo", () => OnVoiceCommand("startGame"));
        keywords.Add("novo jogo", () => OnVoiceCommand("startGame"));
        keywords.Add("iniciar", () => OnVoiceCommand("startGame"));
        keywords.Add("começar", () => OnVoiceCommand("startGame"));
        keywords.Add("jogar", () => OnVoiceCommand("startGame"));
        keywords.Add("start", () => OnVoiceCommand("startGame"));
        keywords.Add("play", () => OnVoiceCommand("startGame"));
        keywords.Add("new game", () => OnVoiceCommand("startGame"));
        keywords.Add("start game", () => OnVoiceCommand("startGame"));
        
        keywords.Add("opções", () => OnVoiceCommand("options"));
        keywords.Add("configurações", () => OnVoiceCommand("options"));
        keywords.Add("settings", () => OnVoiceCommand("options"));
        keywords.Add("options", () => OnVoiceCommand("options"));
        keywords.Add("config", () => OnVoiceCommand("options"));
        
        keywords.Add("créditos", () => OnVoiceCommand("credits"));
        keywords.Add("sobre", () => OnVoiceCommand("credits"));
        keywords.Add("credits", () => OnVoiceCommand("credits"));
        keywords.Add("about", () => OnVoiceCommand("credits"));
        keywords.Add("info", () => OnVoiceCommand("credits"));
        
        keywords.Add("sair", () => OnVoiceCommand("quit"));
        keywords.Add("fechar", () => OnVoiceCommand("quit"));
        keywords.Add("encerrar", () => OnVoiceCommand("quit"));
        keywords.Add("quit", () => OnVoiceCommand("quit"));
        keywords.Add("exit", () => OnVoiceCommand("quit"));
        keywords.Add("close", () => OnVoiceCommand("quit"));

        Debug.Log($"MenuVoiceController: {keywords.Count} palavras-chave adicionadas.");
    }
    
    void StartVoiceRecognition()
    {
        Debug.Log($"MenuVoiceController: Tentando iniciar reconhecimento com {keywords.Count} palavras-chave.");

        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            Debug.LogWarning("MenuVoiceController: Reconhecimento de voz já está rodando. Ignorando Start().");
            return;
        }

        if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Start();
            Debug.Log("MenuVoiceController: Reconhecimento de voz reiniciado.");
            return;
        }

        if (keywordRecognizer == null)
        {
            if (keywords.Count == 0)
            {
                Debug.LogError("MenuVoiceController: Não há palavras-chave para iniciar o reconhecimento. Certifique-se de que SetupVoiceCommands() foi chamado e adicionou palavras-chave.");
                return;
            }
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();
            Debug.Log("MenuVoiceController: Reconhecimento de voz iniciado pela primeira vez.");
        }
    }
    
    void StopVoiceRecognition()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            Debug.Log("MenuVoiceController: Reconhecimento de voz parado");
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

        if (recognizedConfidenceValue >= confidenceThreshold)
        {
            Debug.Log($"MenuVoiceController: Comando de voz reconhecido: \'{args.text}\' (Confiança: {args.confidence})");
            
            if (keywords.ContainsKey(args.text))
            {
                keywords[args.text].Invoke();
            }
        }
        else
        {
            Debug.Log($"MenuVoiceController: Comando de voz ignorado por baixa confiança: \'{args.text}\' (Confiança: {args.confidence})");
        }
    }
    
    void OnVoiceCommand(string command)
    {
        Debug.Log($"MenuVoiceController: OnVoiceCommand chamado para: {command}");
        switch (command)
        {
            case "startGame":
                voiceStartGame = true;
                Debug.Log("MenuVoiceController: voiceStartGame setado para TRUE.");
                break;
                
            case "options":
                voiceOptions = true;
                Debug.Log("MenuVoiceController: voiceOptions setado para TRUE.");
                break;
                
            case "credits":
                voiceCredits = true;
                Debug.Log("MenuVoiceController: voiceCredits setado para TRUE.");
                break;
                
            case "quit":
                voiceQuit = true;
                Debug.Log("MenuVoiceController: voiceQuit setado para TRUE.");
                break;
        }
    }
    
    void Update()
    {
        if (voiceStartGame)
        {
            Debug.Log("MenuVoiceController: Executando StartGame()");
            if (menuController != null)
            {
                menuController.StartGame();
            }
            voiceStartGame = false; 
        }
        
        if (voiceOptions)
        {
            Debug.Log("MenuVoiceController: Executando OptionsScene()");
            if (menuController != null)
            {
                menuController.OptionsScene();
            }
            voiceOptions = false; 
        }
        
        if (voiceCredits)
        {
            Debug.Log("MenuVoiceController: Executando CreditsScene()");
            if (menuController != null)
            {
                menuController.CreditsScene();
            }
            voiceCredits = false; 
        }
        
        if (voiceQuit)
        {
            Debug.Log("MenuVoiceController: Executando leaveGame()");
            if (menuController != null)
            {
                menuController.leaveGame();
            }
            voiceQuit = false; 
        }
    }
    
    public void EnableVoiceCommands()
    {
        enableVoiceCommands = true;
        Debug.Log("MenuVoiceController: Comandos de voz habilitados.");
        StartVoiceRecognition();
    }
    
    public void DisableVoiceCommands()
    {
        enableVoiceCommands = false;
        Debug.Log("MenuVoiceController: Comandos de voz desabilitados.");
        StopVoiceRecognition();
        
        voiceStartGame = false;
        voiceOptions = false;
        voiceCredits = false;
        voiceQuit = false;
        Debug.Log("MenuVoiceController: Comandos de voz ativos resetados.");
    }
    
    public void SetConfidenceThreshold(float threshold)
    {
        confidenceThreshold = Mathf.Clamp01(threshold);
        Debug.Log($"MenuVoiceController: Confidence Threshold ajustado para: {confidenceThreshold}");
    }
    
    void OnDestroy()
    {
        StopVoiceRecognition();
        
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Dispose();
            Debug.Log("MenuVoiceController: KeywordRecognizer disposed.");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            StopVoiceRecognition();
            Debug.Log("MenuVoiceController: Aplicação pausada, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("MenuVoiceController: Aplicação despausada, reconhecimento reiniciado.");
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            StopVoiceRecognition();
            Debug.Log("MenuVoiceController: Aplicação perdeu foco, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("MenuVoiceController: Aplicação ganhou foco, reconhecimento reiniciado.");
        }
    }
}

