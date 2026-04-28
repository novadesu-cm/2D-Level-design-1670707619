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
    public GameObject shieldModel;
    public bool isShielding = false;
    public bool isShieldBroken = false;
    public float shieldCooldown = 2f;

    [Header("ตำแหน่งการถือโล่ (Visuals)")]
    public Vector3 shieldIdlePosition = new Vector3(-0.5f, 0f, 0f);
    public Vector3 shieldIdleRotation = new Vector3(0f, 90f, 20f);

    public Vector3 shieldBlockPosition = new Vector3(0f, 0.5f, 0.8f);
    public Vector3 shieldBlockRotation = new Vector3(0f, 0f, 0f);

    public float shieldRaiseSpeed = 15f;

    [Header("UI ข้อความแจ้งเตือน")]
    public GameObject shieldWarningText;

    [Header("ระบบเสียง (Audio)")]
    public AudioClip hurtSound;        // 🎵 เสียงโดนโจมตีเข้าเนื้อ
    public AudioClip shieldBreakSound; // 🎵 เสียงตอนโล่แตก
    public AudioClip shieldRaiseSound; // 🎵 เสียงตอนยกโล่ขึ้นมาบล็อก (ใหม่!)
    private AudioSource audioSource;

    private PlayerMovement movement;
    private Coroutine warningRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        movement = GetComponent<PlayerMovement>();
        UpdateHealthBar();

        if (shieldModel != null)
        {
            shieldModel.SetActive(true);
            shieldModel.transform.localPosition = shieldIdlePosition;
            shieldModel.transform.localRotation = Quaternion.Euler(shieldIdleRotation);
        }

        if (shieldWarningText != null) shieldWarningText.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        bool holdingShieldInput = Input.GetButton("Fire2") || Input.GetMouseButton(1);

        if (holdingShieldInput)
        {
            if (!isShieldBroken)
            {
                // เช็คว่า "เพิ่งจะยกโล่ขึ้นมาใช่ไหม?" (ก่อนหน้านี้ไม่ได้ยก)
                if (!isShielding)
                {
                    // 🔊 เล่นเสียงยกโล่ตรงนี้!
                    if (shieldRaiseSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(shieldRaiseSound);
                    }
                }

                isShielding = true;
                if (movement != null) movement.SetShieldMovement(true);
                HideWarningImmediately();
            }
            else
            {
                isShielding = false;
                if (movement != null) movement.SetShieldMovement(false);
                ShowShieldWarning();
            }
        }
        else
        {
            isShielding = false;
            if (movement != null) movement.SetShieldMovement(false);
        }

        if (shieldModel != null && !isShieldBroken)
        {
            Vector3 targetPos = isShielding ? shieldBlockPosition : shieldIdlePosition;
            Quaternion targetRot = Quaternion.Euler(isShielding ? shieldBlockRotation : shieldIdleRotation);

            shieldModel.transform.localPosition = Vector3.Lerp(shieldModel.transform.localPosition, targetPos, Time.deltaTime * shieldRaiseSpeed);
            shieldModel.transform.localRotation = Quaternion.Slerp(shieldModel.transform.localRotation, targetRot, Time.deltaTime * shieldRaiseSpeed);
        }
    }

    void ShowShieldWarning()
    {
        if (shieldWarningText != null && !shieldWarningText.activeSelf)
        {
            if (warningRoutine != null) StopCoroutine(warningRoutine);
            warningRoutine = StartCoroutine(ShieldWarningRoutine());
        }
    }

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
        yield return new WaitForSeconds(1f);
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

        if (hurtSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtSound);
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

        // 🔊 เล่นเสียงโล่แตกตรงนี้!
        if (shieldBreakSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shieldBreakSound);
        }

        if (shieldModel != null) shieldModel.SetActive(false);
        if (movement != null) movement.SetShieldMovement(false);

        StartCoroutine(ShieldCooldownRoutine());
    }

    IEnumerator ShieldCooldownRoutine()
    {
        yield return new WaitForSeconds(shieldCooldown);
        isShieldBroken = false;

        if (shieldModel != null)
        {
            shieldModel.SetActive(true);
            shieldModel.transform.localPosition = shieldIdlePosition;
            shieldModel.transform.localRotation = Quaternion.Euler(shieldIdleRotation);
        }

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