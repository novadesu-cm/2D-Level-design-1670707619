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

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity; // สำหรับแรงแนวตั้ง (กระโดด/แรงโน้มถ่วง)

    // สถานะแดช
    private bool isDashing = false;
    private float dashEndTime = 0f;
    private float nextDashTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
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
        }
    }

    void NormalMovement()
    {
        // 1. เช็คพื้นและรีเซ็ตแรงโน้มถ่วง
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. คำนวณทิศทางการเดิน (แนวราบ X, Z)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDir = Vector3.zero; // สร้างตัวแปรมารับค่าการเดิน

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir = moveDir.normalized * moveSpeed; // เก็บค่าความเร็วเดินไว้ก่อน ยังไม่สั่ง Move
        }

        // 3. คำนวณการกระโดด (แนวตั้ง Y)
        // สังเกตว่าเราใช้ controller.isGrounded ตรงนี้ได้แม่นยำขึ้นแล้ว
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 4. คำนวณแรงโน้มถ่วง
        velocity.y += gravity * Time.deltaTime;

        // 5. นำแนวราบและแนวตั้งมารวมกัน แล้วสั่ง Move ครั้งเดียว!
        Vector3 finalMovement = (moveDir * Time.deltaTime) + (velocity * Time.deltaTime);
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
}