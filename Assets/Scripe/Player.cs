using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Double Jump Settings")]
    public int maxJumps = 2;       // จำนวนครั้งที่กระโดดได้ (พื้น 1 + กลางอากาศ 1)
    private int jumpCount = 0;     // ตัวนับจำนวนครั้งที่กระโดดไปแล้ว

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;

    [Header("Respawn Settings")]
    public Vector3 currentRespawnPosition;

    [Header("References")]
    public Transform cameraTransform;

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
        // 🛑 รีเซ็ตจำนวนการกระโดดเมื่อแตะพื้น
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
            jumpCount = 0; // แตะพื้นปุ๊บ รีเซ็ตให้กระโดดได้ใหม่ 2 ทีทันที
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

        // 🚀 ระบบ Double Jump
        // เงื่อนไข: กด Spacebar + (อยู่บนพื้น OR กระโดดไปแล้วไม่เกิน maxJumps) + ห้ามกางโล่อยู่
        if (Input.GetButtonDown("Jump") && !isShieldingWalk)
        {
            if (controller.isGrounded || jumpCount < maxJumps - 1)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount++; // เพิ่มจำนวนครั้งที่กระโดด
                Debug.Log("กระโดดครั้งที่: " + jumpCount);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);

        Vector3 finalMovement = (moveDir * Time.deltaTime) + (velocity * Time.deltaTime) + (knockbackVelocity * Time.deltaTime);
        controller.Move(finalMovement);
    }

    // --- ส่วนแดชและระบบเดิม (คงไว้เหมือนเดิม) ---
    void HandleDashInput()
    {
        if (isShieldingWalk) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;
            velocity.y = 0f;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            currentDashDir = (inputDir.magnitude >= 0.1f) ?
                (transform.right * horizontal + transform.forward * vertical).normalized : transform.forward;
        }
    }

    void DashMovement() { if (Time.time < dashEndTime) controller.Move(currentDashDir * dashSpeed * Time.deltaTime); else isDashing = false; }
    void HandleInteractInput() { /* โค้ดเปิดประตูเดิมของคุณ */ }
    public void ApplyBounce(Vector3 bounceDir, float force) { velocity.y = bounceDir.normalized.y * force; knockbackVelocity = new Vector3(bounceDir.x, 0, bounceDir.z).normalized * force; }
    public void Respawn(Vector3 pos) { controller.enabled = false; transform.position = pos; velocity = Vector3.zero; controller.enabled = true; }
    public void Die() { Respawn(currentRespawnPosition); }
    public void SetShieldMovement(bool isShielding) { isShieldingWalk = isShielding; }
}