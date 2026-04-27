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

    // 🛑 ตัวแปรใหม่: เช็คว่ากำลังเดินเต่า (กางโล่) อยู่ไหม
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
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveInput = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = Vector3.zero;

        // 🐢 ระบบเดินเต่า: ถ้ากางโล่อยู่ ความเร็วจะเหลือแค่ 30% ของปกติ
        float currentMoveSpeed = isShieldingWalk ? moveSpeed * 0.3f : moveSpeed;

        if (moveInput.magnitude >= 0.1f)
        {
            moveDir = transform.right * horizontal + transform.forward * vertical;
            moveDir = moveDir.normalized * currentMoveSpeed;
        }

        // ห้ามกระโดดด้วยตอนกางโล่
        if (Input.GetButtonDown("Jump") && controller.isGrounded && !isShieldingWalk)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);

        Vector3 finalMovement = (moveDir * Time.deltaTime) + (velocity * Time.deltaTime) + (knockbackVelocity * Time.deltaTime);
        controller.Move(finalMovement);
    }

    void HandleDashInput()
    {
        // 🛑 ห้ามแดชตอนกางโล่เด็ดขาด!
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

            if (inputDir.magnitude >= 0.1f)
            {
                currentDashDir = (transform.right * horizontal + transform.forward * vertical).normalized;
            }
            else
            {
                currentDashDir = transform.forward;
            }
        }
    }

    void DashMovement()
    {
        if (Time.time < dashEndTime)
        {
            controller.Move(currentDashDir * dashSpeed * Time.deltaTime);
        }
        else
        {
            isDashing = false;
        }
    }

    void HandleInteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3f))
            {
                DoorUnlock door = hit.collider.GetComponent<DoorUnlock>();
                if (door != null) door.TryUnlock(gameObject);
            }
        }
    }

    public void ApplyBounce(Vector3 bounceDir, float force)
    {
        velocity.y = bounceDir.normalized.y * force;
        Vector3 horizontalDir = new Vector3(bounceDir.x, 0, bounceDir.z).normalized;
        knockbackVelocity = horizontalDir * force;
    }

    public void Respawn(Vector3 respawnPosition)
    {
        controller.enabled = false;
        transform.position = respawnPosition;
        velocity = Vector3.zero;
        knockbackVelocity = Vector3.zero;
        controller.enabled = true;
    }

    public void Die()
    {
        Respawn(currentRespawnPosition);
    }

    // 🛡️ ฟังก์ชันให้สคริปต์เลือดเรียกใช้เพื่อสั่งให้เดินเต่า
    public void SetShieldMovement(bool isShielding)
    {
        isShieldingWalk = isShielding;
    }
}