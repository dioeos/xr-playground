
//using UnityEngine;

//public class BillboardUI : MonoBehaviour
//{
//    [SerializeField]
//    private Transform targetCamera;

//    private void Start()
//    {
//        if (targetCamera == null && Camera.main != null)
//        {
//            targetCamera = Camera.main.transform;
//        }
//    }

//    private void LateUpdate()
//    {
//        if (targetCamera == null)
//        {
//            if (Camera.main != null)
//                targetCamera = Camera.main.transform;
//            else
//                return;
//        }

//        Vector3 direction = targetCamera.position - transform.position;

//        // Keep upright
//        direction.y = 0f;

//        if (direction.sqrMagnitude > 0.0001f)
//        {
//            // Face camera, then rotate 180 degrees around Y
//            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f, 0f);
//        }
//    }
//}

using UnityEngine;

public class PositionUI : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;     // The object you're grabbing (CubeInteractable)

    [SerializeField]
    private Transform targetCamera;     // XR camera

    [Header("Offsets (in meters)")]
    public float forwardOffset = 0.25f; // how far in front of the object (towards user)
    public float sideOffset = 0.15f;    // how far to the side
    public float heightOffset = 0.10f;  // slight vertical offset

    private void Start() {
        if (targetCamera == null && Camera.main != null) {
            targetCamera = Camera.main.transform;
        }
    }

    private void LateUpdate() {
        if (targetObject == null || targetCamera == null) {
            return;
        }

        // Direction from object to camera, flattened on Y so it stays upright
        Vector3 toCamera = targetCamera.position - targetObject.position;
        toCamera.y = 0f;
        if (toCamera.sqrMagnitude < 0.0001f) {
            return;
        }

        Vector3 forward = toCamera.normalized;                  // "front" towards user
        Vector3 right = Vector3.Cross(Vector3.up, forward);     // "to the side" relative to user

        // Compute desired position
        Vector3 basePos = targetObject.position;
        Vector3 offset =
            forward * forwardOffset +   // in front of object (towards user)
            right * sideOffset +        // a bit to the side
            Vector3.up * heightOffset;  // slight height bump

        transform.position = basePos + offset;

        // Face the user
        transform.rotation = Quaternion.LookRotation(forward);
    }
}
