using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lerpRotationPct;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    private Vector2 moveInput;
    private int SpeedID;

    void Start()
    {
        SpeedID = Animator.StringToHash("Speed");
    }

    void Update()
    {
        float speed = 0.0f;

        Vector3 forwardCamera = cameraTransform.forward.normalized;
        Vector3 rightCamera = cameraTransform.right.normalized;

        Vector3 movement = Vector3.zero;

        movement = forwardCamera * moveInput.y + rightCamera * moveInput.x;
        movement.y = 0.0f;
        movement.Normalize();

        float movementSpeed = 0.0f;
        if (moveInput != Vector2.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, lerpRotationPct * Time.deltaTime);

            speed = 1f;
            movementSpeed = walkSpeed;
        }

        animator.SetFloat(SpeedID, speed);
        movement = movement * 800 * Time.deltaTime;
        rb.velocity = movement;
    }
    
    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
