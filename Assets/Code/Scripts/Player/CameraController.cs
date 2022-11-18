using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference lookInput;
    public Transform LookTransform;
    public float distance = 15.0f;
    public float yawRotationalSpeed = 720.0f;
    public float pitchRotationalSpeed = 160.0f;

    float pitch = 0.0f;

    public float minPitch = 30.0f;
    public float maxPitch = -60.0f;

    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;

    [Header("MouseSettings")]
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    public LayerMask avoidObjectsLayer;
    public float avoidObjectsOffset;

    public float minDistance;
    public float maxDistance;
    
    private bool cameraLocked;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;
    }

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.LookAt(LookTransform.position);
        float distance = Vector3.Distance(transform.position, LookTransform.position);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        Vector3 eulerAngels = transform.rotation.eulerAngles;
        float yaw = eulerAngels.y;

        yaw += mouseX * yawRotationalSpeed * Time.deltaTime;
        pitch += mouseY * pitchRotationalSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


        Vector3 l_ForwardCamera = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad), Mathf.Sin(pitch * Mathf.Deg2Rad),
        Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad));
        //transform.position = LookTransform.position - l_ForwardCamera * distance;
        //transform.LookAt(LookTransform.position);
        Vector3 l_DesiredPosition = LookTransform.transform.position - l_ForwardCamera * distance;

        Ray l_Ray = new Ray(LookTransform.position, -l_ForwardCamera);
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, distance, avoidObjectsLayer.value))
            l_DesiredPosition = l_RaycastHit.point + l_ForwardCamera * avoidObjectsOffset;

        transform.position = l_DesiredPosition;
        transform.LookAt(LookTransform.position);
    }
#if UNITY_EDITOR
    public void LockMouse()
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
}
