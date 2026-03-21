using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChargeAI : MonoBehaviour
{
    [Header("Target & Range")]
    public Transform player; // ลากโมเดล Player มาใส่ช่องนี้
    public float aggroRange = 10f; // ระยะมองเห็นที่จะเริ่มวิ่งไล่

    [Header("Movement Settings")]
    public float moveSpeed = 8f; // ความเร็วตอนพุ่งชาร์จ
    public float chaseDuration = 2f; // ระยะเวลาที่วิ่งไล่ก่อนจะนิ่งไป (วินาที)
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    private bool isChasing = false;
    private bool hasFinishedChasing = false; // สถานะเช็คว่าชาร์จเสร็จแล้วหรือยัง
    private float chaseTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. จัดการแรงโน้มถ่วงให้ศัตรูติดพื้นเสมอ
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        // 2. ถ้านิ่งไปแล้ว (วิ่งครบ 2 วิแล้ว) ก็ให้รับแค่แรงโน้มถ่วงอย่างเดียว ไม่ต้องเดินต่อ
        if (hasFinishedChasing)
        {
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        // 3. คำนวณระยะห่างระหว่างศัตรูกับเพลเยอร์
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 4. ตรวจสอบว่าเพลเยอร์เข้ามาในระยะหรือยัง
        if (distanceToPlayer <= aggroRange && !isChasing)
        {
            isChasing = true; // เริ่มพุ่งชาร์จ!
        }

        // 5. ระบบวิ่งไล่และจับเวลา
        if (isChasing)
        {
            chaseTimer += Time.deltaTime; // เริ่มจับเวลา

            if (chaseTimer <= chaseDuration)
            {
                // หันหน้าหาเพลเยอร์ (ล็อคแกน Y ไว้ไม่ให้หน้าศัตรูทิ่มลงพื้นตามตัวเพลเยอร์)
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);

                // เคลื่อนที่พุ่งเข้าหา
                Vector3 moveDir = direction * moveSpeed;

                // รวมการเดินและแรงโน้มถ่วงเข้าด้วยกัน
                controller.Move((moveDir + velocity) * Time.deltaTime);
            }
            else
            {
                // วิ่งครบ 2 วินาทีแล้ว สั่งให้หยุดนิ่งเป็นการถาวร
                hasFinishedChasing = true;
                isChasing = false;
            }
        }
        else
        {
            // กรณียังไม่เข้าระยะ ก็ให้ยืนติดพื้นไว้เฉยๆ
            controller.Move(velocity * Time.deltaTime);
        }
    }

    // ฟังก์ชันพิเศษ: วาดเส้นรัศมีสีแดงในหน้าต่าง Scene เพื่อให้คุณกะระยะง่ายๆ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}