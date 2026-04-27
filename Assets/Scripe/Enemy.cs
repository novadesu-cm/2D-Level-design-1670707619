using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class EnemyChargeAI : MonoBehaviour
{
    [Header("Target & Range")]
    public Transform player;
    public float aggroRange = 10f;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float gravity = -9.81f;

    [Header("Attack Settings (พุ่งโจมตี)")]
    public float attackDamage = 15f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;
    public float windUpTime = 0.5f;
    public float lungeSpeed = 20f;

    [Header("Hit Reaction (ชะงักตอนโดนดาบ)")]
    public float hitStunDuration = 0.4f;

    [Header("Shield Reaction (มึนเมื่อชนโล่)")]
    public float shieldStunDuration = 2f;      // 🕒 ชนโล่แล้วจะมึนนานกว่าโดนฟัน
    public Color stunnedColor = Color.blue;    // 🎨 สีตอนติดมึนชนโล่
    public float bounceBackForce = 15f;        // 💥 แรงเด้งถอยหลังตอนชนโล่

    [Header("Visual Feedback (สีตัวละคร)")]
    public Renderer enemyMesh;
    public Color warningColor = Color.red;
    private Color originalColor;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 knockbackVelocity = Vector3.zero;

    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isStunned = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (enemyMesh != null)
        {
            originalColor = enemyMesh.material.color;
        }
    }

    void Update()
    {
        // 1. จัดการแรงโน้มถ่วง
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        // ค่อยๆ ลดแรงกระเด็นลงเรื่อยๆ
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);

        // 2. ถ้าติดสถานะมึนงง หรือ กำลังโจมตีอยู่ จะไม่เดินไล่ตาม (รับแค่แรงกระเด็นกับแรงโน้มถ่วง)
        if (isStunned || isAttacking)
        {
            controller.Move((velocity + knockbackVelocity) * Time.deltaTime);
            return;
        }

        // 3. คำนวณระยะห่าง
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= aggroRange && !isChasing)
        {
            isChasing = true;
        }

        // 4. ตัดสินใจว่าจะพุ่งโจมตี หรือวิ่งไล่ตาม
        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackLungeRoutine());
        }
        else if (isChasing)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            Vector3 moveDir = direction * moveSpeed;
            controller.Move((moveDir + velocity + knockbackVelocity) * Time.deltaTime);
        }
        else
        {
            controller.Move((velocity + knockbackVelocity) * Time.deltaTime);
        }
    }

    // 💥 ระบบพุ่งโจมตี
    IEnumerator AttackLungeRoutine()
    {
        isAttacking = true;

        // เปลี่ยนเป็นสีแดงเตือน
        if (enemyMesh != null) enemyMesh.material.color = warningColor;

        // หันหน้าล็อกเป้า
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        // รอให้ผู้เล่นเตรียมหลบหรือกางโล่
        yield return new WaitForSeconds(windUpTime);

        // คืนสีเดิม แล้วพุ่งตัว
        if (enemyMesh != null) enemyMesh.material.color = originalColor;

        float lungeDuration = 0.2f;
        float timer = 0f;
        bool hasHit = false;

        while (timer < lungeDuration)
        {
            timer += Time.deltaTime;
            controller.Move((transform.forward * lungeSpeed + velocity) * Time.deltaTime);

            // เช็คการชนเพลเยอร์
            if (!hasHit && Vector3.Distance(transform.position, player.position) <= 1.5f)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // 🛡️ เช็คว่าผู้เล่นยกโล่อยู่หรือไม่?
                    if (playerHealth.isShielding)
                    {
                        hasHit = true;
                        playerHealth.BreakShield(); // สั่งโล่ผู้เล่นแตกและติดคูลดาวน์
                        ShieldStun();               // มอนสเตอร์ติดมึน!
                        break;                      // ยกเลิกการพุ่งทันที
                    }
                    else
                    {
                        // ถ้าไม่ได้กางโล่ โดนดาเมจเต็มๆ
                        playerHealth.TakeDamage(attackDamage);
                        hasHit = true;
                    }
                }
            }
            yield return null;
        }

        // ถ้าระหว่างพุ่งไม่ได้ชนโล่ (ไม่ติด Stun) ก็ให้พักเหนื่อยตามปกติ
        if (!isStunned)
        {
            yield return new WaitForSeconds(attackCooldown);
            isAttacking = false;
        }
    }

    // 🛡️ ระบบชนโล่แล้วมึนงง
    public void ShieldStun()
    {
        StopAllCoroutines(); // หยุดลูปการพุ่งโจมตี
        StartCoroutine(ShieldStunRoutine());
    }

    IEnumerator ShieldStunRoutine()
    {
        isStunned = true;
        isAttacking = false;

        // เปลี่ยนเป็นสีมึนงง (เช่น สีฟ้า)
        if (enemyMesh != null) enemyMesh.material.color = stunnedColor;
        Debug.Log("มอนสเตอร์พุ่งชนโล่! ติดมึนงงอย่างหนัก!");

        // กระเด็นถอยหลังอย่างแรง
        Vector3 bounceDir = (transform.position - player.position).normalized;
        bounceDir.y = 0;
        knockbackVelocity = bounceDir * bounceBackForce;

        // รอมันหายมึน
        yield return new WaitForSeconds(shieldStunDuration);

        // คืนสีเดิมและกลับมาล่าต่อ
        if (enemyMesh != null) enemyMesh.material.color = originalColor;
        isStunned = false;
    }

    // ⚔️ ฟังก์ชันโดนดาบฟันปกติ (มี Hyper Armor)
    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction * force;

        // ถ้าง้างพุ่งอยู่ หรือกำลังพุ่ง จะมีเกราะ Hyper Armor ตีไม่ชะงัก
        if (isAttacking)
        {
            return;
        }

        // แต่ถ้าเดินอยู่เฉยๆ จะชะงักได้
        StopAllCoroutines();
        StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;
        isAttacking = false;

        if (enemyMesh != null) enemyMesh.material.color = originalColor;

        yield return new WaitForSeconds(hitStunDuration);

        isStunned = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}