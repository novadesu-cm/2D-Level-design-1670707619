using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;
    private bool hasAirDashed = false; // 📌 ตัวเก็บข้อมูลว่าใช้สิทธิ์แดชกลางอากาศไปหรือยัง

    [Header("Respawn Settings")]
    public Vector3 currentRespawnPosition;

    [Header("References")]
    public Transform cameraTransform;

    [Header("ระบบเสียง (Audio)")]
    public AudioClip jumpSound;
    public AudioClip dashSound;
    private AudioSource audioSource;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 knockbackVelocity = Vector3.zero;

    private bool isDashing = false;
    private float dashEndTime = 0f;
    private float nextDashTime = 0f;
    private Vector3 currentDashDir;
    private bool isShieldingWalk = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentRespawnPosition = transform.position;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (cameraTransform != null)
        {
            float targetAngle = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        if (isDashing)
        {
            DashMovement();
        }
        else
        {
            NormalMovement();
            HandleDashInput();
        }
    }

    void NormalMovement()
    {
        // 1. เช็คพื้น: คืนสิทธิ์แดชกลางอากาศเมื่อเท้าแตะพื้น
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            hasAirDashed = false; // 🔄 เท้าแตะพื้นปุ๊บ คืนโควต้าแดชให้ทันที
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveInput = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = Vector3.zero;

        float currentMoveSpeed = isShieldingWalk ? moveSpeed * 0.3f : moveSpeed;

        if (moveInput.magnitude >= 0.1f)
        {
            moveDir = transform.right * horizontal + transform.forward * vertical;
            moveDir = moveDir.normalized * currentMoveSpeed;
        }

        // 2. ระบบกระโดด 1 จังหวะ
        if (Input.GetButtonDown("Jump") && !isShieldingWalk)
        {
            if (controller.isGrounded) // ต้องยืนอยู่บนพื้นเท่านั้นถึงจะกระโดดได้
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);

        Vector3 finalMovement = (moveDir * Time.deltaTime) + (velocity * Time.deltaTime) + (knockbackVelocity * Time.deltaTime);
        controller.Move(finalMovement);
    }

    void HandleDashInput()
    {
        if (isShieldingWalk) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            // 🛑 เช็คว่าถ้าตัวลอยอยู่ และเคยแดชไปแล้ว ให้กดไม่ติด
            if (!controller.isGrounded && hasAirDashed)
            {
                return;
            }

            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;
            velocity.y = 0f; // ล็อกความสูงไว้ ทำให้แดชพุ่งตรงๆ กลางอากาศไม่ตกลงมา

            // ถ้าแดชตอนลอยอยู่ ให้ริบโควต้า (hasAirDashed เป็น true)
            if (!controller.isGrounded)
            {
                hasAirDashed = true;
            }

            if (dashSound != null) audioSource.PlayOneShot(dashSound);

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            currentDashDir = (inputDir.magnitude >= 0.1f) ?
                (transform.right * horizontal + transform.forward * vertical).normalized : transform.forward;
        }
    }

    void DashMovement()
    {
        if (Time.time < dashEndTime)
            controller.Move(currentDashDir * dashSpeed * Time.deltaTime);
        else
            isDashing = false;
    }

    public void ApplyBounce(Vector3 bounceDir, float force)
    {
        velocity.y = bounceDir.normalized.y * force;
        knockbackVelocity = new Vector3(bounceDir.x, 0, bounceDir.z).normalized * force;
        hasAirDashed = false; // ทริคเสริม: คืนสิทธิ์แดชให้เผื่อกระเด้งกับดักแล้วอยากพุ่งหลบ
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        direction.y = 0;
        knockbackVelocity = direction.normalized * force;
        velocity.y = 3f;
    }

    public void Respawn(Vector3 pos)
    {
        controller.enabled = false;
        transform.position = pos;
        velocity = Vector3.zero;
        knockbackVelocity = Vector3.zero;
        hasAirDashed = false;
        controller.enabled = true;
    }

    public void Die()
    {
        Respawn(currentRespawnPosition);
    }

    public void SetShieldMovement(bool isShielding)
    {
        isShieldingWalk = isShielding;
    }
}