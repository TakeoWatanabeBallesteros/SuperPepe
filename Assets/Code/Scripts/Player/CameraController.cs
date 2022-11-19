using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private Transform lookTransform;
    [SerializeField] private float distance = 15.0f;
    [SerializeField] private float yawRotationalSpeed = 720.0f;
    [SerializeField] private float pitchRotationalSpeed = 160.0f;

    private float pitch = 0.0f;

    [SerializeField] private float minPitch = 30.0f;
    [SerializeField] private float maxPitch = -60.0f;

    [SerializeField] private LayerMask avoidObjectsLayer;
    [SerializeField] private float avoidObjectsOffset;

    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    
    private bool cameraLocked;
    private Vector2 mouseInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;
    }

    private void LateUpdate()
    {
        transform.LookAt(lookTransform.position);
        var distance = Vector3.Distance(transform.position, lookTransform.position);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        var yaw = transform.rotation.eulerAngles.y;

        yaw += mouseInput.x * yawRotationalSpeed * Time.deltaTime;
        pitch += mouseInput.y * pitchRotationalSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


        var forwardCamera = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad), 
                                        Mathf.Sin(pitch * Mathf.Deg2Rad),
                                        Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad));
        
        var desiredPosition = lookTransform.position - forwardCamera * distance;

        var l_Ray = new Ray(lookTransform.position, -forwardCamera);
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, distance, avoidObjectsLayer.value))
            desiredPosition = l_RaycastHit.point + forwardCamera * avoidObjectsOffset;

        transform.position = desiredPosition;
        transform.LookAt(lookTransform.position);
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
