using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class VoiceController : MonoBehaviour
{
    [Header("Voice Recognition Settings")]
    public float confidenceThreshold = 0.5f;
    public bool enableVoiceCommands = true;
    
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    
    private PlayerController playerController;
    
    [HideInInspector] public bool voiceMoveRight = false;
    [HideInInspector] public bool voiceMoveLeft = false;
    [HideInInspector] public bool voiceJump = false;
    [HideInInspector] public bool voiceShoot = false;
    
    private float moveCommandDuration = 0.5f; 
    private float moveRightTimer = 0f;
    private float moveLeftTimer = 0f;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        if (playerController == null)
        {
            Debug.LogError("VoiceController: PlayerController não encontrado!");
        }
        
        SetupVoiceCommands();
        
        if (enableVoiceCommands)
        {
            StartVoiceRecognition();
        }
    }
    
    void SetupVoiceCommands()
    {
        Debug.Log("VoiceController: Entrando em SetupVoiceCommands().");

        keywords.Clear();

        // Comandos em português
        keywords.Add("direita", () => OnVoiceCommand("moveRight"));
        keywords.Add("esquerda", () => OnVoiceCommand("moveLeft"));
        keywords.Add("pular", () => OnVoiceCommand("jump"));
        keywords.Add("atirar", () => OnVoiceCommand("shoot"));
        keywords.Add("fogo", () => OnVoiceCommand("shoot"));
        
        // Comandos alternativos
        keywords.Add("andar direita", () => OnVoiceCommand("moveRight"));
        keywords.Add("andar esquerda", () => OnVoiceCommand("moveLeft"));
        keywords.Add("ir direita", () => OnVoiceCommand("moveRight"));
        keywords.Add("ir esquerda", () => OnVoiceCommand("moveLeft"));
        keywords.Add("saltar", () => OnVoiceCommand("jump"));
        keywords.Add("disparar", () => OnVoiceCommand("shoot"));
        
        // Comandos em inglês
        keywords.Add("right", () => OnVoiceCommand("moveRight"));
        keywords.Add("left", () => OnVoiceCommand("moveLeft"));
        keywords.Add("jump", () => OnVoiceCommand("jump"));
        keywords.Add("shoot", () => OnVoiceCommand("shoot"));
        keywords.Add("fire", () => OnVoiceCommand("shoot"));
        
        // Comandos de parada
        keywords.Add("parar", () => OnVoiceCommand("stop"));
        keywords.Add("stop", () => OnVoiceCommand("stop"));

        Debug.Log($"VoiceController: {keywords.Count} palavras-chave adicionadas.");
    }
    
    void StartVoiceRecognition()
    {
        Debug.Log($"VoiceController: Tentando iniciar reconhecimento com {keywords.Count} palavras-chave.");

        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            Debug.LogWarning("VoiceController: Reconhecimento de voz já está rodando. Ignorando Start().");
            return;
        }

        if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Start();
            Debug.Log("VoiceController: Reconhecimento de voz reiniciado.");
            return;
        }

        if (keywordRecognizer == null)
        {
            if (keywords.Count == 0)
            {
                Debug.LogError("VoiceController: Não há palavras-chave para iniciar o reconhecimento. Certifique-se de que SetupVoiceCommands() foi chamado e adicionou palavras-chave.");
                return; 
            }
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();
            Debug.Log("VoiceController: Reconhecimento de voz iniciado pela primeira vez.");
        }
    }
    
    void StopVoiceRecognition()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            Debug.Log("VoiceController: Reconhecimento de voz parado");
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
            Debug.Log($"VoiceController: Comando de voz reconhecido: \'{args.text}\' (Confiança: {args.confidence})");

            if (keywords.ContainsKey(args.text))
            {
                keywords[args.text].Invoke();
            }
        }
        else
        {
            Debug.Log($"VoiceController: Comando de voz ignorado por baixa confiança: \'{args.text}\' (Confiança: {args.confidence})");
        }
    }
    
    void OnVoiceCommand(string command)
    {
        Debug.Log($"VoiceController: OnVoiceCommand chamado para: {command}");
        switch (command)
        {
            case "moveRight":
                voiceMoveRight = true;
                moveRightTimer = moveCommandDuration;
                voiceMoveLeft = false; 
                break;
                
            case "moveLeft":
                voiceMoveLeft = true;
                moveLeftTimer = moveCommandDuration;
                voiceMoveRight = false; 
                break;
                
            case "jump":
                voiceJump = true;
                Debug.Log("VoiceController: voiceJump setado para TRUE.");
                break;
                
            case "shoot":
                voiceShoot = true;
                Debug.Log("VoiceController: voiceShoot setado para TRUE.");
                break;
                
            case "stop":
                voiceMoveRight = false;
                voiceMoveLeft = false;
                moveRightTimer = 0f;
                moveLeftTimer = 0f;
                break;
        }
    }
    
    void Update()
    {
        if (moveRightTimer > 0f)
        {
            moveRightTimer -= Time.deltaTime;
            if (moveRightTimer <= 0f)
            {
                voiceMoveRight = false;
                Debug.Log("VoiceController: voiceMoveRight setado para FALSE (timer expirou).");
            }
        }
        
        if (moveLeftTimer > 0f)
        {
            moveLeftTimer -= Time.deltaTime;
            if (moveLeftTimer <= 0f)
            {
                voiceMoveLeft = false;
                Debug.Log("VoiceController: voiceMoveLeft setado para FALSE (timer expirou).");
            }
        }

    }
    
    public void EnableVoiceCommands()
    {
        enableVoiceCommands = true;
        Debug.Log("VoiceController: Comandos de voz habilitados.");
        StartVoiceRecognition();
    }
    
    public void DisableVoiceCommands()
    {
        enableVoiceCommands = false;
        Debug.Log("VoiceController: Comandos de voz desabilitados.");
        StopVoiceRecognition();
        
        voiceMoveRight = false;
        voiceMoveLeft = false;
        voiceJump = false;
        voiceShoot = false;
        moveRightTimer = 0f;
        moveLeftTimer = 0f;
        Debug.Log("VoiceController: Comandos de voz ativos resetados.");
    }
    
    public void SetConfidenceThreshold(float threshold)
    {
        confidenceThreshold = Mathf.Clamp01(threshold);
        Debug.Log($"VoiceController: Confidence Threshold ajustado para: {confidenceThreshold}");
    }
    
    public void SetMoveCommandDuration(float duration)
    {
        moveCommandDuration = Mathf.Max(0.1f, duration);
        Debug.Log($"VoiceController: Move Command Duration ajustado para: {moveCommandDuration}");
    }
    
    void OnDestroy()
    {
        StopVoiceRecognition();
        
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Dispose();
            Debug.Log("VoiceController: KeywordRecognizer disposed.");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            StopVoiceRecognition();
            Debug.Log("VoiceController: Aplicação pausada, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("VoiceController: Aplicação despausada, reconhecimento reiniciado.");
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            StopVoiceRecognition();
            Debug.Log("VoiceController: Aplicação perdeu foco, reconhecimento parado.");
        }
        else if (enableVoiceCommands)
        {
            StartVoiceRecognition();
            Debug.Log("VoiceController: Aplicação ganhou foco, reconhecimento reiniciado.");
        }
    }
}

