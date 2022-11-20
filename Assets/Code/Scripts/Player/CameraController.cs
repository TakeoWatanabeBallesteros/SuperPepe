using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject thirdPersonCharacterBase;
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
        distance = Vector3.Distance(transform.position, lookAtTransform.position);
        // float newOpacity = (actualDistance - shadowOnlyDistance) / (ditherDistance - minCameraDistance);
        // float lerpPosition = transitionTime > 0 ? deltaTime * 1 / transitionTime : 1;
        // previousOpacity = Mathf.Lerp(previousOpacity, newOpacity, lerpPosition);
        // Set opacity of character based on how close the camera is
        RecursiveSetFloatProperty(thirdPersonCharacterBase, "_Opacity", distance > 1.0f ? 1.0f : Scale(0.6f, 1.0f, distance));
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
    
    /// <summary>
    /// Recursively set a float property for each Renderer components'
    /// materials for a given object and its children.
    /// </summary>
    /// <param name="original">Base game object to start operation from.</param>
    /// <param name="property">Name of property to modify.</param>
    /// <param name="value">Value to set for float property.</param>
    /// <param name="sharedMaterial">Should the shared materials be modified.</param>
    private static void RecursiveSetFloatProperty(GameObject original, string property, float value, bool sharedMaterial = false)
    {
        foreach (SkinnedMeshRenderer renderer in original.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material mat in sharedMaterial ? renderer.sharedMaterials : renderer.materials)
            {
                mat.SetFloat(property, value);
            }
        }
    }
    private static float Scale(float min, float max, float value)
    {
        return Mathf.Clamp(1 / (max - min) * (value - max) + 1, 0, 1);
    }
}
