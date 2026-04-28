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

    [Header("Player Knockback (แรงกระเด็นของผู้เล่น)")]
    public float knockbackForceToPlayer = 15f; // 💥 ปรับความแรงที่ผู้เล่นจะกระเด็นตรงนี้ได้เลย

    [Header("Hit Reaction (ชะงักตอนโดนดาบ)")]
    public float hitStunDuration = 0.4f;

    [Header("Shield Reaction (มึนเมื่อชนโล่)")]
    public float shieldStunDuration = 2f;
    public Color stunnedColor = Color.blue;
    public float bounceBackForce = 15f;

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
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);

        if (isStunned || isAttacking)
        {
            controller.Move((velocity + knockbackVelocity) * Time.deltaTime);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= aggroRange && !isChasing)
        {
            isChasing = true;
        }

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

    IEnumerator AttackLungeRoutine()
    {
        isAttacking = true;

        if (enemyMesh != null) enemyMesh.material.color = warningColor;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        yield return new WaitForSeconds(windUpTime);

        if (enemyMesh != null) enemyMesh.material.color = originalColor;

        float lungeDuration = 0.2f;
        float timer = 0f;
        bool hasHit = false;

        while (timer < lungeDuration)
        {
            timer += Time.deltaTime;
            controller.Move((transform.forward * lungeSpeed + velocity) * Time.deltaTime);

            if (!hasHit && Vector3.Distance(transform.position, player.position) <= 1.5f)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    if (playerHealth.isShielding)
                    {
                        hasHit = true;
                        playerHealth.BreakShield();
                        ShieldStun();
                        break;
                    }
                    else
                    {
                        // 1. ลดเลือดผู้เล่น
                        playerHealth.TakeDamage(attackDamage);
                        hasHit = true;

                        // 2. 👇 สั่งให้ผู้เล่นกระเด็นถอยหลัง
                        PlayerMovement pMove = player.GetComponent<PlayerMovement>();
                        if (pMove != null)
                        {
                            Vector3 kbDir = (player.position - transform.position).normalized;
                            pMove.ApplyKnockback(kbDir, knockbackForceToPlayer);
                        }
                    }
                }
            }
            yield return null;
        }

        if (!isStunned)
        {
            yield return new WaitForSeconds(attackCooldown);
            isAttacking = false;
        }
    }

    public void ShieldStun()
    {
        StopAllCoroutines();
        StartCoroutine(ShieldStunRoutine());
    }

    IEnumerator ShieldStunRoutine()
    {
        isStunned = true;
        isAttacking = false;

        if (enemyMesh != null) enemyMesh.material.color = stunnedColor;

        Vector3 bounceDir = (transform.position - player.position).normalized;
        bounceDir.y = 0;
        knockbackVelocity = bounceDir * bounceBackForce;

        yield return new WaitForSeconds(shieldStunDuration);

        if (enemyMesh != null) enemyMesh.material.color = originalColor;
        isStunned = false;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction * force;

        if (isAttacking) return;

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