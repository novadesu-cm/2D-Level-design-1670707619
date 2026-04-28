using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("ตั้งค่าการโจมตี (Hitbox)")]
    public float attackDamage = 25f;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public float knockbackForce = 15f;
    public float hitDelay = 0.1f;

    [Header("จุดกำเนิดการโจมตี")]
    public Transform attackPoint;
    public Vector3 defaultAttackOffset = new Vector3(0, 1f, 1f);
    public LayerMask enemyLayer;

    [Header("ระบบแกว่งดาบ")]
    public Transform swordModel;
    public Vector3 swingAngle = new Vector3(90f, 0f, 0f);
    public Vector3 swingForwardOffset = new Vector3(0f, 0f, 0.8f);
    public float swingSpeed = 0.15f;

    [Header("ระบบเสียง (Audio)")]
    public AudioClip swingSound; // 🎵 เสียงตอนง้างฟันลม
    public AudioClip hitSound;   // 🎵 เสียงตอนฟันโดนเนื้อ/เกราะมอนสเตอร์
    private AudioSource audioSource;

    private float nextAttackTime = 0f;
    private bool isSwinging = false;
    private Quaternion swordStartRotation;
    private Vector3 swordStartPosition;

    void Start()
    {
        // สร้างลำโพงที่ตัวผู้เล่นอัตโนมัติ
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (swordModel != null)
        {
            swordStartRotation = swordModel.localRotation;
            swordStartPosition = swordModel.localPosition;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            StartCoroutine(PerformAttackRoutine());
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    IEnumerator PerformAttackRoutine()
    {
        if (swordModel != null && !isSwinging)
        {
            StartCoroutine(SwingSword());
        }

        // 🔊 เล่นเสียงฟันลมทันทีที่กดตี
        if (swingSound != null) audioSource.PlayOneShot(swingSound);

        yield return new WaitForSeconds(hitDelay);

        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.TransformDirection(defaultAttackOffset);
        Collider[] hitEnemies = Physics.OverlapSphere(origin, attackRange, enemyLayer);

        bool hasHitSomeone = false;

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                hasHitSomeone = true; // บันทึกไว้ว่าฟันโดนศัตรูแล้ว
            }

            EnemyChargeAI enemyAI = enemy.GetComponent<EnemyChargeAI>();
            if (enemyAI != null)
            {
                Vector3 knockbackDir = (enemy.transform.position - transform.position).normalized;
                knockbackDir.y = 0;
                enemyAI.ApplyKnockback(knockbackDir, knockbackForce);
            }
        }

        // 🔊 ถ้าฟันโดนศัตรู ให้เล่นเสียงเนื้อกระทบดาบ
        if (hasHitSomeone && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    IEnumerator SwingSword()
    {
        isSwinging = true;
        float time = 0f;
        Quaternion targetRotation = swordStartRotation * Quaternion.Euler(swingAngle);
        Vector3 targetPosition = swordStartPosition + swingForwardOffset;

        while (time < swingSpeed)
        {
            time += Time.deltaTime;
            float percent = time / swingSpeed;
            swordModel.localRotation = Quaternion.Slerp(swordStartRotation, targetRotation, percent);
            swordModel.localPosition = Vector3.Lerp(swordStartPosition, targetPosition, percent);
            yield return null;
        }

        time = 0f;
        while (time < swingSpeed)
        {
            time += Time.deltaTime;
            float percent = time / swingSpeed;
            swordModel.localRotation = Quaternion.Slerp(targetRotation, swordStartRotation, percent);
            swordModel.localPosition = Vector3.Lerp(targetPosition, swordStartPosition, percent);
            yield return null;
        }

        swordModel.localRotation = swordStartRotation;
        swordModel.localPosition = swordStartPosition;
        isSwinging = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.TransformDirection(defaultAttackOffset);
        Gizmos.DrawSphere(origin, attackRange);
    }
}