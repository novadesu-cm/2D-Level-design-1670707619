using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChargeAI : MonoBehaviour
{
    [Header("Target & Range")]
    public Transform player;
    public float aggroRange = 10f;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float chaseDuration = 2f;
    public float gravity = -9.81f;

    [Header("Attack Settings (เพิ่มใหม่)")]
    public float attackDamage = 15f;    // แรงตบของมอนสเตอร์
    public float attackRange = 1.5f;     // ระยะที่มอนสเตอร์จะตบถึง
    public float attackCooldown = 1f;  // ตบเสร็จแล้วพักกี่วินาทีถึงจะตบใหม่ได้
    private float nextAttackTime = 0f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 knockbackVelocity = Vector3.zero;

    private bool isChasing = false;
    private bool hasFinishedChasing = false;
    private float chaseTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // --- ระบบโจมตีผู้เล่น ---
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }

        if (hasFinishedChasing)
        {
            controller.Move((velocity + knockbackVelocity) * Time.deltaTime);
            return;
        }

        if (distanceToPlayer <= aggroRange && !isChasing)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            chaseTimer += Time.deltaTime;

            if (chaseTimer <= chaseDuration)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);

                Vector3 moveDir = direction * moveSpeed;
                controller.Move((moveDir + velocity + knockbackVelocity) * Time.deltaTime);
            }
            else
            {
                hasFinishedChasing = true;
                isChasing = false;
            }
        }
        else
        {
            controller.Move((velocity + knockbackVelocity) * Time.deltaTime);
        }
    }

    void AttackPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("มอนสเตอร์ตบเพลเยอร์!");
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction * force;
    }

    private void OnDrawGizmosSelected()
    {
        // วาดวงกลมสีเหลืองแสดงระยะตบในหน้า Scene
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}