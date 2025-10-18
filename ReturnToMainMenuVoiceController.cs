using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class ReturnToMainMenuVoiceController : MonoBehaviour
{
    [Header("Voice Recognition Settings")]
    public float confidenceThreshold = 0.5f;
    public bool enableVoiceCommands = true;
    
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    
    private ReturnToMainMenu returnToMainMenuController;
    
    void Start()
    {
        returnToMainMenuController = GetComponent<ReturnToMainMenu>();
        if (returnToMainMenuController == null)
        {
            returnToMainMenuController = FindObjectOfType<ReturnToMainMenu>();
        }
        
        if (returnToMainMenuController == null)
        {
            Debug.LogError("ReturnToMainMenuVoiceController: ReturnToMainMenu não encontrado!");
            return;
        }
        else
        {
            Debug.Log("ReturnToMainMenuVoiceController: ReturnToMainMenu encontrado com sucesso!");
        }
        
        SetupVoiceCommands();
        
        if (enableVoiceCommands)
        {
            StartVoiceRecognition();
        }
    }
    
    void SetupVoiceCommands()
    {
        Debug.Log("ReturnToMainMenuVoiceController: Entrando em SetupVoiceCommands().");

        keywords.Clear();

        // Comandos para RETORNAR AO INÍCIO / MENU PRINCIPAL
        keywords.Add("retornar ao início", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("retornar ao inicio", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("voltar ao início", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("voltar ao inicio", () => OnVoiceCommand("returnToMainMenu"));

        keywords.Add("ir para o menu", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("menu", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("início", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("inicio", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("voltar", () => OnVoiceCommand("returnToMainMenu"));
        
        // Comandos em inglês
        keywords.Add("return to start", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("back to start", () => OnVoiceCommand("returnToMainMenu"));

        keywords.Add("go to menu", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("back to menu", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("start", () => OnVoiceCommand("returnToMainMenu"));
        keywords.Add("back", () => OnVoiceCommand("returnToMainMenu"));

        Debug.Log($"ReturnToMainMenuVoiceController: {keywords.Count} palavras-chave adicionadas. Palavras-chave: {string.Join(", ", keywords.Keys.ToArray())}");
    }
    
    void StartVoiceRecognition()
    {
        Debug.Log($"ReturnToMainMenuVoiceController: Tentando iniciar reconhecimento com {keywords.Count} palavras-chave. KeywordRecognizer atual: {(keywordRecognizer == null ? "null" : (keywordRecognizer.IsRunning ? "rodando" : "parado"))}");

        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            Debug.LogWarning("ReturnToMainMenuVoiceController: Reconhecimento de voz já está rodando. Ignorando Start().");
            return;
        }

        if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Start();
            Debug.Log("ReturnToMainMenuVoiceController: Reconhecimento de voz reiniciado.");
            return;
        }

        if (keywordRecognizer == null)
        {
            if (keywords.Count == 0)
            {
                Debug.LogError("ReturnToMainMenuVoiceController: Não há palavras-chave para iniciar o reconhecimento. Certifique-se de que SetupVoiceCommands() foi chamado e adicionou palavras-chave.");
                return;
            }
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();
            Debug.Log("ReturnToMainMenuVoiceController: Reconhecimento de voz iniciado pela primeira vez.");
        }
    }
    
    void StopVoiceRecognition()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            Debug.Log("ReturnToMainMenuVoiceController: Reconhecimento de voz parado");
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
            Debug.Log($"ReturnToMainMenuVoiceController: Comando de voz reconhecido: \'{args.text}\' (Confiança: {args.confidence}, Valor: {recognizedConfidenceValue:F2}, Threshold: {confidenceThreshold:F2})");
            
            if (keywords.ContainsKey(args.text))
            {
                keywords[args.text].Invoke();
            }
        }
        else
        {
            Debug.Log($"ReturnToMainMenuVoiceController: Comando de voz ignorado por baixa confiança: \'{args.text}\' (Confiança: {args.confidence})");
        }
    }
    
    void OnVoiceCommand(string command)
    {
        Debug.Log($"ReturnToMainMenuVoiceController: OnVoiceCommand chamado para: {command}");
        switch (command)
        {
            case "returnToMainMenu":
                if (returnToMainMenuController != null)
                {
                    Debug.Log("ReturnToMainMenuVoiceController: Executando ReturnToMainMenu.ReturnToMainMenuScene()");
                    returnToMainMenuController.ReturnToMainMenuScene();
                }
                else
                {
                    Debug.LogError("ReturnToMainMenuVoiceController: ReturnToMainMenu não referenciado para 'returnToMainMenu' command.");
                }
                break;
        }
    }
    
    public void EnableVoiceCommands()
    {
        enableVoiceCommands = true;
        Debug.Log("ReturnToMainMenuVoiceController: Comandos de voz habilitados.");
        StartVoiceRecognition();
    }
    
    public void DisableVoiceCommands()
    {
        enableVoiceCommands = false;
        Debug.Log("ReturnToMainMenuVoiceController: Comandos de voz desabilitados.");
        StopVoiceRecognition();
        Debug.Log("ReturnToMainMenuVoiceController: Comandos de voz ativos resetados.");
    }
    
    public void SetConfidenceThreshold(float threshold)
    {
        confidenceThreshold = Mathf.Clamp01(threshold);
        Debug.Log($"ReturnToMainMenuVoiceController: Confidence Threshold ajustado para: {confidenceThreshold}");
    }
    
    void OnDestroy()
    {
        StopVoiceRecognition();
        
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Dispose();
            Debug.Log("ReturnToMainMenuVoiceController: KeywordRecognizer disposed.");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            StopVoiceRecognition();
            Debug.Log("ReturnToMainMenuVoiceController: Aplicação pausada, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("ReturnToMainMenuVoiceController: Aplicação despausada, reconhecimento reiniciada.");
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            StopVoiceRecognition();
            Debug.Log("ReturnToMainMenuVoiceController: Aplicação perdeu foco, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("ReturnToMainMenuVoiceController: Aplicação ganhou foco, reconhecimento reiniciada.");
        }
    }
}
