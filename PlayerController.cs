using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private float horizontal;

    private CharacterController controller;

    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded;

    private bool isFacingRight = true;

    private Animator anim;

    public Transform firePos;
    public float shootRange = 50f;
    public LayerMask targetLayer;

    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;

    public float timeBetweenShots;
    private float shootCounter;
    
    private VoiceController voiceController;
    
    private bool keyboardInput = false;
    private bool voiceInput = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        
        voiceController = GetComponent<VoiceController>();
        if (voiceController == null)
        {
            voiceController = FindObjectOfType<VoiceController>();
        }
        
        if (voiceController == null)
        {
            Debug.LogWarning("PlayerController: VoiceController não encontrado. Comandos de voz não estarão disponíveis.");
        } else {
            Debug.Log("PlayerController: VoiceController encontrado com sucesso!");
        }
    }

    void Update()
    {
        if (!Pause.instance.isPaused)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
            Debug.Log($"PlayerController: isGrounded = {isGrounded}");

            HandleMovementInput();
            HandleJumpInput();
            HandleShootInput();

            Vector3 move = new Vector3(0, 0, horizontal);
            controller.Move(move * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (horizontal > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (horizontal < 0 && isFacingRight)
            {
                Flip();
            }

            if (shootCounter > 0)
            {
                shootCounter -= Time.deltaTime;
            }
        }

        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(horizontal));
    }
    
    void HandleMovementInput()
    {
        // Input do teclado
        float keyboardHorizontal = Input.GetAxisRaw("Horizontal");
        keyboardInput = Mathf.Abs(keyboardHorizontal) > 0;
        
        // Input de voz
        float voiceHorizontal = 0f;
        voiceInput = false;
        
        if (voiceController != null)
        {
            if (voiceController.voiceMoveRight)
            {
                voiceHorizontal = 1f;
                voiceInput = true;
                Debug.Log("PlayerController: Comando de voz para DIREITA detectado.");
            }
            else if (voiceController.voiceMoveLeft)
            {
                voiceHorizontal = -1f;
                voiceInput = true;
                Debug.Log("PlayerController: Comando de voz para ESQUERDA detectado.");
            }
        }
        
        // Prioridade: se há input do teclado, usa o teclado; senão usa a voz
        if (keyboardInput)
        {
            horizontal = keyboardHorizontal * moveSpeed;
            Debug.Log($"PlayerController: Input de teclado (Horizontal: {keyboardHorizontal}) priorizado.");
        }
        else if (voiceInput)
        {
            horizontal = voiceHorizontal * moveSpeed;
            Debug.Log($"PlayerController: Input de voz (Horizontal: {voiceHorizontal}) aplicado.");
        }
        else
        {
            horizontal = 0f;
        }

        if (horizontal != 0f) {
            Debug.Log($"PlayerController: Movimento horizontal final: {horizontal}");
        }
    }
    
    void HandleJumpInput()
    {
        bool keyboardJump = Input.GetButtonDown("Jump");
        bool voiceJump = false;
        
        if (voiceController != null)
        {
            voiceJump = voiceController.voiceJump;
        }
        
        if (voiceJump) {
            Debug.Log("PlayerController: Comando de voz para PULAR detectado. voiceJump = TRUE.");
        }

        if ((keyboardJump || voiceJump) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            AudioManager.instance.PlaySfx(3);
            Debug.Log("PlayerController: Pulo executado!");
            
            if (voiceController != null) voiceController.voiceJump = false;
        }
        else if (voiceJump && !isGrounded)
        {
            Debug.Log("PlayerController: Comando de voz para PULAR detectado, mas não está no chão (isGrounded = FALSE).");
            
            if (voiceController != null) voiceController.voiceJump = false;
        }
    }
    
    void HandleShootInput()
    {
        bool keyboardShoot = Input.GetMouseButtonDown(0);
        bool voiceShoot = false;
        
        if (voiceController != null)
        {
            voiceShoot = voiceController.voiceShoot;
        }
        
        if (voiceShoot) {
            Debug.Log("PlayerController: Comando de voz para ATIRAR detectado. voiceShoot = TRUE.");
        }

        if ((keyboardShoot || voiceShoot) && shootCounter <= 0)
        {
            Shoot();
            AudioManager.instance.PlaySfx(4);
            anim.SetTrigger("Shoot");
            shootCounter = timeBetweenShots;
            Debug.Log("PlayerController: Tiro executado!");
            
            if (voiceController != null) voiceController.voiceShoot = false;
        }
        else if (voiceShoot && shootCounter > 0)
        {
            Debug.Log($"PlayerController: Comando de voz para ATIRAR detectado, mas em cooldown (shootCounter = {shootCounter:F2}).");
            
            if (voiceController != null) voiceController.voiceShoot = false;
        }
    }

    //Cálculo para inverter o lado
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.z *= -1;
        transform.localScale = localScale;
        Debug.Log($"PlayerController: Player virado para {(isFacingRight ? "Direita" : "Esquerda")}.");
    }

    void Shoot()
    {
        Vector3 shootDirection;

        if (isFacingRight)
        {
            shootDirection = firePos.forward;
        }
        else
        {
            shootDirection = -firePos.forward;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePos.position, firePos.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = shootDirection * projectileSpeed;

        RaycastHit hit;
        if (Physics.Raycast(firePos.position, shootDirection, out hit, shootRange, targetLayer))
        {
            Debug.Log("PlayerController: Raio de tiro atingiu: " + hit.collider.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    
    public void EnableVoiceCommands()
    {
        if (voiceController != null)
        {
            voiceController.EnableVoiceCommands();
            Debug.Log("PlayerController: Solicitando habilitação de comandos de voz.");
        }
    }
    
    public void DisableVoiceCommands()
    {
        if (voiceController != null)
        {
            voiceController.DisableVoiceCommands();
            Debug.Log("PlayerController: Solicitando desabilitação de comandos de voz.");
        }
    }
    
    public bool IsUsingVoiceInput()
    {
        return voiceInput;
    }
    
    public bool IsUsingKeyboardInput()
    {
        return keyboardInput;
    }
}
