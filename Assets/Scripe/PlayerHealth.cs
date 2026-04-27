using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("ตั้งค่าพลังชีวิต")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;

    [Header("ระบบโล่ป้องกัน")]
    public bool isShielding = false;
    public bool isShieldBroken = false;
    public float shieldCooldown = 2f;
    public GameObject shieldModel;

    [Header("UI ข้อความแจ้งเตือน")]
    public GameObject shieldWarningText; // ลาก Text UI มาใส่

    private PlayerMovement movement;
    private Coroutine warningRoutine; // เก็บค่า Coroutine ไว้เพื่อสั่งหยุดได้แม่นยำ

    void Start()
    {
        currentHealth = maxHealth;
        movement = GetComponent<PlayerMovement>();
        UpdateHealthBar();

        if (shieldModel != null) shieldModel.SetActive(false);
        if (shieldWarningText != null) shieldWarningText.SetActive(false);
    }

    void Update()
    {
        // 1. ตรวจสอบการกดปุ่มโล่ (คลิกขวา)
        bool holdingShieldInput = Input.GetButton("Fire2") || Input.GetMouseButton(1);

        if (holdingShieldInput)
        {
            if (!isShieldBroken)
            {
                // ✅ กรณีโล่พร้อมใช้
                isShielding = true;
                if (shieldModel != null) shieldModel.SetActive(true);
                if (movement != null) movement.SetShieldMovement(true);

                // ถ้าโล่พร้อมใช้แล้วแต่ข้อความยังค้างอยู่ ให้ปิดทิ้งทันที
                HideWarningImmediately();
            }
            else
            {
                // ❌ กรณีโล่ยังไม่พร้อม (แตกอยู่)
                isShielding = false;
                if (shieldModel != null) shieldModel.SetActive(false);
                if (movement != null) movement.SetShieldMovement(false);

                // โชว์ข้อความเตือน
                ShowShieldWarning();
            }
        }
        else
        {
            // 🚫 กรณีไม่ได้กดปุ่ม
            isShielding = false;
            if (shieldModel != null) shieldModel.SetActive(false);
            if (movement != null) movement.SetShieldMovement(false);
        }
    }

    // ฟังก์ชันสั่งแสดงข้อความเตือน
    void ShowShieldWarning()
    {
        // ถ้าข้อความปิดอยู่ ให้เริ่มโชว์
        if (shieldWarningText != null && !shieldWarningText.activeSelf)
        {
            if (warningRoutine != null) StopCoroutine(warningRoutine);
            warningRoutine = StartCoroutine(ShieldWarningRoutine());
        }
    }

    // ฟังก์ชันปิดข้อความทันที (ใช้ตอนโล่ซ่อมเสร็จหรือเริ่มกางโล่ได้)
    void HideWarningImmediately()
    {
        if (shieldWarningText != null && shieldWarningText.activeSelf)
        {
            if (warningRoutine != null) StopCoroutine(warningRoutine);
            shieldWarningText.SetActive(false);
        }
    }

    IEnumerator ShieldWarningRoutine()
    {
        shieldWarningText.SetActive(true);
        yield return new WaitForSeconds(1f); // โชว์ค้างไว้ 1 วินาที
        shieldWarningText.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (isShielding)
        {
            Debug.Log("โล่รับดาเมจแทน! โล่แตก!");
            BreakShield();
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0) Die();
    }

    public void BreakShield()
    {
        if (isShieldBroken) return;
        isShieldBroken = true;
        isShielding = false;

        if (shieldModel != null) shieldModel.SetActive(false);
        if (movement != null) movement.SetShieldMovement(false);

        StartCoroutine(ShieldCooldownRoutine());
    }

    IEnumerator ShieldCooldownRoutine()
    {
        yield return new WaitForSeconds(shieldCooldown);
        isShieldBroken = false;

        // ✨ เมื่อซ่อมเสร็จ ให้ปิดข้อความเตือนทันที ไม่ต้องรอให้ครบวิ
        HideWarningImmediately();
        Debug.Log("ซ่อมโล่เสร็จแล้ว!");
    }

    void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.value = currentHealth / maxHealth;
    }

    void Die()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        if (movement != null) movement.Die();
    }
}