using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class PauseVoiceController : MonoBehaviour
{
    [Header("Voice Recognition Settings")]
    public float confidenceThreshold = 0.5f;
    public bool enableVoiceCommands = true;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    private Pause pauseController;

    [HideInInspector] public bool voicePauseUnpause = false;
    [HideInInspector] public bool voiceMenu = false;
    [HideInInspector] public bool voiceRestart = false;

    private bool isInitialized = false;

    void Start()
    {
        pauseController = GetComponent<Pause>();
        if (pauseController == null)
        {
            pauseController = FindObjectOfType<Pause>();
        }

        if (pauseController == null)
        {
            Debug.LogError("PauseVoiceController: Pause não encontrado! Comandos de voz para o menu de pausa não funcionarão.");
            return;
        }
        else
        {
            Debug.Log("PauseVoiceController: Pause encontrado com sucesso!");
        }

        SetupVoiceCommands();

        if (enableVoiceCommands)
        {
            StartVoiceRecognition();
        }

        isInitialized = true;
        Debug.Log("PauseVoiceController: Componente inicializado.");
    }

    void SetupVoiceCommands()
    {
        Debug.Log("PauseVoiceController: Entrando em SetupVoiceCommands().");

        keywords.Clear();

        // Comandos para pausar o jogo
        keywords.Add("pausar", () => OnVoiceCommand("pauseUnpause"));
        keywords.Add("pause", () => OnVoiceCommand("pauseUnpause"));
        keywords.Add("resumir", () => OnVoiceCommand("resume"));
        keywords.Add("continuar", () => OnVoiceCommand("resume"));
        keywords.Add("voltar", () => OnVoiceCommand("resume"));
        keywords.Add("despausar", () => OnVoiceCommand("resume"));
        keywords.Add("resume", () => OnVoiceCommand("resume"));
        keywords.Add("continue", () => OnVoiceCommand("resume"));
        keywords.Add("resumo", () => OnVoiceCommand("resume"));

        // Comandos para voltar ao menu principal
        keywords.Add("menu principal", () => OnVoiceCommand("menu"));
        keywords.Add("menu", () => OnVoiceCommand("menu"));
        keywords.Add("sair", () => OnVoiceCommand("menu"));
        keywords.Add("main menu", () => OnVoiceCommand("menu"));
        keywords.Add("exit", () => OnVoiceCommand("menu"));
        keywords.Add("quit", () => OnVoiceCommand("menu"));

        // Comandos para reiniciar a fase
        keywords.Add("reiniciar", () => OnVoiceCommand("restart"));
        keywords.Add("recomeçar", () => OnVoiceCommand("restart"));
        keywords.Add("começar de novo", () => OnVoiceCommand("restart"));
        keywords.Add("restart", () => OnVoiceCommand("restart"));
        keywords.Add("reset", () => OnVoiceCommand("restart"));

        Debug.Log($"PauseVoiceController: {keywords.Count} palavras-chave adicionadas.");
    }

    void StartVoiceRecognition()
    {
        Debug.Log($"PauseVoiceController: Tentando iniciar reconhecimento com {keywords.Count} palavras-chave.");

        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            Debug.LogWarning("PauseVoiceController: Reconhecimento de voz já está rodando. Ignorando Start().");
            return;
        }

        if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Start();
            Debug.Log("PauseVoiceController: Reconhecimento de voz reiniciado.");
            return;
        }

        if (keywordRecognizer == null)
        {
            if (keywords.Count == 0)
            {
                Debug.LogError("PauseVoiceController: Não há palavras-chave para iniciar o reconhecimento. Certifique-se de que SetupVoiceCommands() foi chamado e adicionou palavras-chave.");
                return;
            }
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();
            Debug.Log("PauseVoiceController: Reconhecimento de voz iniciado pela primeira vez.");
        }
    }

    void StopVoiceRecognition()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            Debug.Log("PauseVoiceController: Reconhecimento de voz parado");
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
            Debug.Log($"PauseVoiceController: Comando de voz reconhecido: '{args.text}' (Confiança: {args.confidence})");
            if (keywords.ContainsKey(args.text))
            {
                keywords[args.text].Invoke();
            }
        }
        else
        {
            Debug.Log($"PauseVoiceController: Comando de voz ignorado por baixa confiança: '{args.text}' (Confiança: {args.confidence})");
        }
    }

    void OnVoiceCommand(string command)
    {
        Debug.Log($"PauseVoiceController: OnVoiceCommand chamado para: {command}");
        switch (command)
        {
            case "pauseUnpause":
                if (pauseController != null && !pauseController.isPaused)
                {
                    voicePauseUnpause = true;
                    Debug.Log("PauseVoiceController: voicePauseUnpause setado para TRUE (pausar).");
                }
                else if (pauseController != null && pauseController.isPaused)
                {
                    Debug.Log("PauseVoiceController: Jogo já pausado, ignorando comando 'pausar'. Use 'resumir'.");
                }
                break;
            case "resume":
                if (pauseController != null && pauseController.isPaused)
                {
                    voicePauseUnpause = true;
                    Debug.Log("PauseVoiceController: voicePauseUnpause setado para TRUE (despausar).");
                }
                else if (pauseController != null && !pauseController.isPaused)
                {
                    Debug.Log("PauseVoiceController: Jogo não pausado, ignorando comando 'resumir'. Use 'pausar'.");
                }
                break;
            case "menu":
                voiceMenu = true;
                Debug.Log("PauseVoiceController: voiceMenu setado para TRUE.");
                break;
            case "restart":
                voiceRestart = true;
                Debug.Log("PauseVoiceController: voiceRestart setado para TRUE.");
                break;
        }
    }

    void Update()
    {
        if (voicePauseUnpause)
        {
            Debug.Log("PauseVoiceController: Executando PauseUnpause()");
            if (pauseController != null)
            {
                pauseController.PauseUnpause();
            }
            voicePauseUnpause = false;
        }

        if (voiceMenu)
        {
            Debug.Log("PauseVoiceController: Executando MenuButton()");
            if (pauseController != null)
            {
                pauseController.MenuButton();
            }
            voiceMenu = false;
        }

        if (voiceRestart)
        {
            Debug.Log("PauseVoiceController: Executando RestartButton()");
            if (pauseController != null)
            {
                pauseController.RestartButton();
            }
            voiceRestart = false;
        }
    }

    public void EnableVoiceCommands()
    {
        enableVoiceCommands = true;
        Debug.Log("PauseVoiceController: Comandos de voz habilitados.");
        StartVoiceRecognition();
    }

    public void DisableVoiceCommands()
    {
        enableVoiceCommands = false;
        Debug.Log("PauseVoiceController: Comandos de voz desabilitados.");
        StopVoiceRecognition();

        voicePauseUnpause = false;
        voiceMenu = false;
        voiceRestart = false;
        Debug.Log("PauseVoiceController: Comandos de voz ativos resetados.");
    }

    public void SetConfidenceThreshold(float threshold)
    {
        confidenceThreshold = Mathf.Clamp01(threshold);
        Debug.Log($"PauseVoiceController: Confidence Threshold ajustado para: {confidenceThreshold}");
    }

    void OnDestroy()
    {
        StopVoiceRecognition();

        if (keywordRecognizer != null)
        {
            keywordRecognizer.Dispose();
            Debug.Log("PauseVoiceController: KeywordRecognizer disposed.");
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!isInitialized) return;

        if (pauseStatus)
        {
            StopVoiceRecognition();
            Debug.Log("PauseVoiceController: Aplicação pausada, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("PauseVoiceController: Aplicação despausada, reconhecimento reiniciado.");
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!isInitialized) return;

        if (!hasFocus)
        {
            StopVoiceRecognition();
            Debug.Log("PauseVoiceController: Aplicação perdeu foco, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("PauseVoiceController: Aplicação ganhou foco, reconhecimento reiniciado.");
        }
    }
}
