using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.5f, -4f);
    public float mouseSensitivity = 2f;

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        // 👇 ปิดการล็อกเมาส์ตรงนี้ทิ้งไปเลยครับ ให้สคริปต์เมนูเป็นคนจัดการแทน
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        // ดึงมุมหมุน Y (ซ้าย-ขวา) ของตัวละครมาตั้งเป็นมุมเริ่มต้นของกล้อง
        if (target != null)
        {
            currentX = target.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        // 👇 เพิ่มบรรทัดนี้: ถ้าเวลาหยุด (เปิดเมนูอยู่) ให้หยุดการหมุนกล้องทันที!
        if (Time.timeScale == 0f) return;

        if (target == null) return;

        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentY = Mathf.Clamp(currentY, -15f, 60f);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}