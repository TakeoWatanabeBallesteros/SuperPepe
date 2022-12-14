using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject thirdPersonCharacterBase;
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private Transform cameraTransform;
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

    [SerializeField] private Transform idlePos;
    private float idleTimer;

    private Vector2 mouseInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;
    }
    
    public void LateUpdate()
    {
        cameraTransform.LookAt(lookAtTransform.position);
        float distance = Vector3.Distance(cameraTransform.position, lookAtTransform.position);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        
        Vector3 eulerAngles = cameraTransform.rotation.eulerAngles;
        float yaw = eulerAngles.y;

        yaw += mouseInput.x * yawRotationalSpeed * Time.deltaTime;
        pitch += mouseInput.y * pitchRotationalSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 forwardCamera = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad), 
                                              Mathf.Sin(pitch * Mathf.Deg2Rad), 
                                              Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad));
        Vector3 desiredPosition = idleTimer < 5 ? lookAtTransform.transform.position - forwardCamera * distance : idlePos.position;

        Ray ray = new Ray(lookAtTransform.position, -forwardCamera);
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(ray, out l_RaycastHit, distance, avoidObjectsLayerMask.value))
            desiredPosition = l_RaycastHit.point + forwardCamera * avoidObjectsOffset;

        cameraTransform.position = idleTimer < 5 ? desiredPosition : Vector3.Lerp(cameraTransform.position, desiredPosition, Time.deltaTime * 5);
        cameraTransform.LookAt(lookAtTransform.position);
        distance = Vector3.Distance(cameraTransform.position, lookAtTransform.position);
        // Set opacity of character based on how close the camera is
        RecursiveSetFloatProperty(thirdPersonCharacterBase, "_Opacity", distance > 1.0f ? 1.0f : Scale(0.6f, 1.0f, distance));

        if (idleTimer < 5) idleTimer += Time.deltaTime;
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
        idleTimer = 0;
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
