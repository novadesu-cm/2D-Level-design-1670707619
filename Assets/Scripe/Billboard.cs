using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // หากล้องหลักในฉากอัตโนมัติ
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        // สั่งให้ UI หันหน้าตั้งฉากกับกล้องเสมอ
        if (camTransform != null)
        {
            transform.LookAt(transform.position + camTransform.forward);
        }
    }
}