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
    // 👇 ตัวแปรสำคัญ: ต้องมีอันนี้เพื่อจำว่าล่าสุดเซฟที่ไหน
    public Vector3 currentRespawnPosition;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 knockbackVelocity = Vector3.zero;

    private bool isDashing = false;
    private float dashEndTime = 0f;
    private float nextDashTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // 👇 เมื่อเริ่มเกม ให้จำตำแหน่งแรกสุดไว้เป็นจุดเกิด
        currentRespawnPosition = transform.position;
    }

    void Update()
    {
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
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir = moveDir.normalized * moveSpeed;
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;
            velocity.y = 0f;
        }
    }

    void DashMovement()
    {
        if (Time.time < dashEndTime)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
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

    // 👇 ฟังก์ชันวาร์ป (Respawn)
    public void Respawn(Vector3 respawnPosition)
    {
        controller.enabled = false; // ปิด Controller ก่อนย้าย
        transform.position = respawnPosition;
        velocity = Vector3.zero;
        knockbackVelocity = Vector3.zero;
        controller.enabled = true; // ย้ายเสร็จแล้วเปิดใหม่
    }

    // 👇 ฟังก์ชันตาย (Die)
    public void Die()
    {
        Respawn(currentRespawnPosition);
    }
}