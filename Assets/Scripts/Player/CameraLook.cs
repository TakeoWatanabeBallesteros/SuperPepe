using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private Transform lookAt;
    
    
    private bool cameraLocked;
    private float pitch;
    private float yaw;
    
    // Start is called before the first frame update
    private void Start()
    {
#if UNITY_EDITOR
        // mouseLock.action.Enable();
        // mouseLock.action.performed += _ =>
        // {
        //     cameraLocked = !cameraLocked;
        //     if (cameraLocked)
        //     {
        //         lookInput.action.Disable();
        //         Cursor.lockState = CursorLockMode.None;
        //     }
        //     else
        //     {
        //         lookInput.action.Enable();
        //         Cursor.lockState = CursorLockMode.Locked;
        //     }
        //     Cursor.visible = cameraLocked;
        // };
#endif
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        // 
        // yaw = InputX * yawRotationSpeed * Time.deltaTime;
        // pitch = InputY * pitchRotationSpeed * Time*deltaTime;
        // pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        
        // Vector3 cameraForward = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad),
        // Mathf.Sin(pitch * Mathf.Deg2Rad), Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad));
        // transform.position = lookAt.position - cameraForward * Distance;
        // transform.LookAt(lookAt.position);
    }
}
