using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float yawRotationalSpeed;
    [SerializeField] private float pitchRotationalSpeed;

    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;

    private float pitch = 0.0f;

    private bool cameraLocked;

    [Header("Avoid Objects")]
    public LayerMask avoidObjectsLayerMask;
    public float avoidObjectsOffset;

    private Vector2 mouseInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;
    }
    
    public void LateUpdate()
    {
        transform.LookAt(lookAtTransform.position);
        float distance = Vector3.Distance(transform.position, lookAtTransform.position);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        float yaw = eulerAngles.y;

        yaw += mouseInput.x * yawRotationalSpeed * Time.deltaTime;
        pitch += mouseInput.y * pitchRotationalSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 forwardCamera = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad), 
                                              Mathf.Sin(pitch * Mathf.Deg2Rad), 
                                              Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad));
        Vector3 desiredPosition = lookAtTransform.transform.position - forwardCamera * distance;

        Ray ray = new Ray(lookAtTransform.position, -forwardCamera);
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(ray, out l_RaycastHit, distance, avoidObjectsLayerMask.value))
            desiredPosition = l_RaycastHit.point + forwardCamera * avoidObjectsOffset;

        transform.position = desiredPosition;
        transform.LookAt(lookAtTransform.position);
    }
    
#if UNITY_EDITOR
    public void LockMouse(InputAction.CallbackContext context)
    {
        cameraLocked = !cameraLocked;
        if (cameraLocked)
        {
            lookInput.action.Disable();
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            lookInput.action.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = cameraLocked;
    }
#endif

    public void ReadMouseInput(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }
}
