using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("ตั้งค่าการโจมตี (Hitbox)")]
    public float attackDamage = 25f;
    public float attackRange = 1.5f; // ขนาดวงกว้างของรัศมีดาบ
    public float attackCooldown = 0.5f;
    public float knockbackForce = 15f;

    [Tooltip("เวลาที่หน่วงก่อนทำดาเมจ (ให้ตรงกับจังหวะดาบฟาดลงมากลางอากาศ)")]
    public float hitDelay = 0.1f; // 🕒 ตัวแปรใหม่!

    [Header("จุดกำเนิดการโจมตี")]
    public Transform attackPoint; // จุดศูนย์กลาง Hitbox
    public Vector3 defaultAttackOffset = new Vector3(0, 1f, 1f); // ตำแหน่งสำรองถ้าไม่ได้ใส่ attackPoint
    public LayerMask enemyLayer;

    [Header("ระบบแกว่งดาบ")]
    public Transform swordModel;
    public Vector3 swingAngle = new Vector3(90f, 0f, 0f);
    public Vector3 swingForwardOffset = new Vector3(0f, 0f, 0.8f);
    public float swingSpeed = 0.15f;

    private float nextAttackTime = 0f;
    private bool isSwinging = false;
    private Quaternion swordStartRotation;
    private Vector3 swordStartPosition;

    void Start()
    {
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
            // เปลี่ยนมาใช้ Coroutine ควบคุมจังหวะการทำดาเมจแทน
            StartCoroutine(PerformAttackRoutine());
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // Coroutine สำหรับจัดการจังหวะฟันและจังหวะทำดาเมจ
    IEnumerator PerformAttackRoutine()
    {
        // 1. สั่งให้ภาพดาบเริ่มแกว่งทันที
        if (swordModel != null && !isSwinging)
        {
            StartCoroutine(SwingSword());
        }

        // 2. รอเสี้ยววินาทีให้ภาพดาบฟาดลงมาถึงตัวศัตรู
        yield return new WaitForSeconds(hitDelay);

        // 3. เริ่มเช็คระยะและทำดาเมจ (Hitbox)
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.TransformDirection(defaultAttackOffset);
        Collider[] hitEnemies = Physics.OverlapSphere(origin, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }

            EnemyChargeAI enemyAI = enemy.GetComponent<EnemyChargeAI>();
            if (enemyAI != null)
            {
                Vector3 knockbackDir = (enemy.transform.position - transform.position).normalized;
                knockbackDir.y = 0;
                enemyAI.ApplyKnockback(knockbackDir, knockbackForce);
            }
        }
    }

    IEnumerator SwingSword()
    {
        isSwinging = true;
        float time = 0f;

        Quaternion targetRotation = swordStartRotation * Quaternion.Euler(swingAngle);
        Vector3 targetPosition = swordStartPosition + swingForwardOffset;

        // จังหวะฟาดดาบลง
        while (time < swingSpeed)
        {
            time += Time.deltaTime;
            float percent = time / swingSpeed;
            swordModel.localRotation = Quaternion.Slerp(swordStartRotation, targetRotation, percent);
            swordModel.localPosition = Vector3.Lerp(swordStartPosition, targetPosition, percent);
            yield return null;
        }

        time = 0f;
        // จังหวะดึงดาบกลับ
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

    // ฟังก์ชันนี้จะวาดลูกแก้วสีแดงใน Scene ให้คุณกะระยะ Hitbox ได้แบบเป๊ะๆ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f); // สีแดงแบบโปร่งแสง 40%
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.TransformDirection(defaultAttackOffset);
        Gizmos.DrawSphere(origin, attackRange);
    }
}