using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Double Jump Settings")]
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;

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
        Cursor.lockState = CursorLockMode.Locked;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
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
            HandleInteractInput();
        }
    }

    void NormalMovement()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0) velocity.y = -2f;
            jumpCount = 0;
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

        if (Input.GetButtonDown("Jump") && !isShieldingWalk)
        {
            if (controller.isGrounded || jumpCount < maxJumps - 1)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount++;

                if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
            }
        }

        velocity.y += gravity * Time.deltaTime;

        // ค่อยๆ ลดแรงกระเด็นลงเรื่อยๆ
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);

        Vector3 finalMovement = (moveDir * Time.deltaTime) + (velocity * Time.deltaTime) + (knockbackVelocity * Time.deltaTime);
        controller.Move(finalMovement);
    }

    void HandleDashInput()
    {
        if (isShieldingWalk) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;
            velocity.y = 0f;

            if (dashSound != null) audioSource.PlayOneShot(dashSound);

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            currentDashDir = (inputDir.magnitude >= 0.1f) ?
                (transform.right * horizontal + transform.forward * vertical).normalized : transform.forward;
        }
    }

    void DashMovement() { if (Time.time < dashEndTime) controller.Move(currentDashDir * dashSpeed * Time.deltaTime); else isDashing = false; }
    void HandleInteractInput() { }

    public void ApplyBounce(Vector3 bounceDir, float force) { velocity.y = bounceDir.normalized.y * force; knockbackVelocity = new Vector3(bounceDir.x, 0, bounceDir.z).normalized * force; }

    // 👇 ฟังก์ชันใหม่: รับแรงผลักจากการโดนโจมตี
    public void ApplyKnockback(Vector3 direction, float force)
    {
        direction.y = 0; // ไม่เอาแกน Y มาคิด
        knockbackVelocity = direction.normalized * force;
        velocity.y = 3f; // สั่งให้ตัวลอยกระเด้งขึ้นจากพื้นนิดนึงตอนโดนตี
    }

    public void Respawn(Vector3 pos) { controller.enabled = false; transform.position = pos; velocity = Vector3.zero; knockbackVelocity = Vector3.zero; controller.enabled = true; }
    public void Die() { Respawn(currentRespawnPosition); }
    public void SetShieldMovement(bool isShielding) { isShieldingWalk = isShielding; }
}