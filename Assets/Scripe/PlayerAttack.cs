using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("ตั้งค่าการโจมตี")]
    public float attackDamage = 25f;
    public float attackRange = 1.5f; // ระยะการโจมตี
    public float attackCooldown = 0.5f; // ดีเลย์ระหว่างการฟันแต่ละครั้ง

    [Header("จุดกำเนิดการโจมตี")]
    public Transform attackPoint; // จุดที่จะสร้างวงกลมโจมตี (อยู่หน้าผู้เล่น)
    public LayerMask enemyLayer; // กำหนดเลเยอร์เป้าหมายเพื่อไม่ให้ตีโดนกำแพงหรือตัวเอง

    private float nextAttackTime = 0f;

    void Update()
    {
        // เช็คการกดคลิกซ้าย (Fire1) และเช็คว่าพ้นคูลดาวน์หรือยัง
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        // 1. กำหนดจุดศูนย์กลางการโจมตี
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.forward;

        // 2. ตรวจจับวัตถุทั้งหมดในระยะที่เป็น Layer ศัตรู
        Collider[] hitEnemies = Physics.OverlapSphere(origin, attackRange, enemyLayer);

        // 3. วนลูปสร้างความเสียหายให้ศัตรูทุกตัวที่โดนรัศมี
        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log("ฟันโดน: " + enemy.name);
            }
        }
    }

    // วาดรัศมีสีแดงในหน้าต่าง Scene ให้กะระยะฟันง่ายๆ
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}