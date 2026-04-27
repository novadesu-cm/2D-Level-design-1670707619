using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("ตั้งค่าการโจมตี")]
    public float attackDamage = 25f;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public float knockbackForce = 15f; // 💥 ความแรงตอนกระเด็น

    [Header("ระบบแกว่งดาบ")]
    public Transform swordModel; // ลากโมเดลดาบมาใส่ช่องนี้
    public Vector3 swingAngle = new Vector3(90f, 0f, 0f); // องศาที่จะฟาดลงมา 
    public Vector3 swingForwardOffset = new Vector3(0f, 0f, 0.8f); // 👈 ระยะที่ดาบจะพุ่งไปข้างหน้าตอนฟัน (ปรับค่าแกน Z หรือแกนอื่นตามโมเดล)
    public float swingSpeed = 0.15f; // ความเร็วในการฟาด

    [Header("จุดกำเนิดการโจมตี")]
    public Transform attackPoint;
    public LayerMask enemyLayer;

    private float nextAttackTime = 0f;
    private bool isSwinging = false;

    // ตัวแปรจำค่าเดิมของดาบ
    private Quaternion swordStartRotation;
    private Vector3 swordStartPosition;

    void Start()
    {
        // จำมุมและตำแหน่งเริ่มต้นของดาบเอาไว้ เพื่อดึงกลับมาท่าเดิมหลังฟันเสร็จ
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
            Attack();

            // สั่งแกว่งดาบและพุ่งไปข้างหน้า
            if (swordModel != null && !isSwinging)
            {
                StartCoroutine(SwingSword());
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.forward;
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

    // ระบบแกว่งดาบและพุ่งไปข้างหน้า
    IEnumerator SwingSword()
    {
        isSwinging = true;
        float time = 0f;

        // คำนวณเป้าหมายทั้งมุมหมุน (Rotation) และตำแหน่ง (Position)
        Quaternion targetRotation = swordStartRotation * Quaternion.Euler(swingAngle);
        Vector3 targetPosition = swordStartPosition + swingForwardOffset;

        // จังหวะที่ 1: ฟาดดาบลง + พุ่งไปข้างหน้า
        while (time < swingSpeed)
        {
            time += Time.deltaTime;
            float percent = time / swingSpeed; // คำนวณเปอร์เซ็นต์ความคืบหน้า (0 ถึง 1)

            swordModel.localRotation = Quaternion.Slerp(swordStartRotation, targetRotation, percent);
            swordModel.localPosition = Vector3.Lerp(swordStartPosition, targetPosition, percent);
            yield return null;
        }

        time = 0f;
        // จังหวะที่ 2: ดึงดาบกลับมาท่าเตรียม (ตำแหน่งเดิม)
        while (time < swingSpeed)
        {
            time += Time.deltaTime;
            float percent = time / swingSpeed;

            swordModel.localRotation = Quaternion.Slerp(targetRotation, swordStartRotation, percent);
            swordModel.localPosition = Vector3.Lerp(targetPosition, swordStartPosition, percent);
            yield return null;
        }

        // บังคับให้กลับมาตำแหน่งและมุมเริ่มต้นเป๊ะๆ ป้องกันการค่อยๆ เบี้ยวสะสม
        swordModel.localRotation = swordStartRotation;
        swordModel.localPosition = swordStartPosition;

        isSwinging = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}