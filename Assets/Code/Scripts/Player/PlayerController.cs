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
    private int animSpeedID;
    private float _verticalVelocity;

    void Start()
    {
        animSpeedID = Animator.StringToHash("Speed");
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

        animator.SetFloat(animSpeedID, speed);
        movement = movement * 800 * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
        rb.velocity = movement;
    }

    private void JumpAndGravity()
    {
        // if (grounded)
        // {
        //     // reset the fall timeout timer
        //     _fallTimeoutDelta = fallTimeout;
        //     // update animator if using character
        //     if (_hasAnimator)
        //     {
        //         _animator.SetBool(_animIDJump, false);
        //         _animator.SetBool(_animIDFreeFall, false);
        //     }
 
        //     // stop our velocity dropping infinitely when grounded
        //     if (_verticalVelocity < 0.0f)
        //     {
        //         _verticalVelocity = -2f;
        //     }
 
        //     // Jump
        //     if (_input.jump && _jumpTimeoutDelta <= 0.0f)
        //     {
        //         // the square root of H * -2 * G = how much velocity needed to reach desired height
        //         _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        //         // update animator if using character
        //         if (_hasAnimator)
        //         {
        //             _animator.SetBool(_animIDJump, true);
        //         }
        //     }
 
        //     // jump timeout
        //     if (_jumpTimeoutDelta >= 0.0f)
        //     {
        //         _jumpTimeoutDelta -= Time.deltaTime;
        //     }
        // }
        // else
        // {
        //     // reset the jump timeout timer
        //     _jumpTimeoutDelta = jumpTimeout;
        //     // fall timeout
        //     if (_fallTimeoutDelta >= 0.0f)
        //     {
        //         _fallTimeoutDelta -= Time.deltaTime;
        //     }
        //     else
        //     {
        //         // update animator if using character
        //         if (_hasAnimator)
        //         {
        //             _animator.SetBool(_animIDFreeFall, true);
        //         }
        //     }
 
        //     // if we are not grounded, do not jump
        //     _input.jump = false;
        // }
 
        // // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        // if (_verticalVelocity < _terminalVelocity)
        // {
        //     _verticalVelocity += gravity * Time.deltaTime;
        // }
    }

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void AssignAnimationIDs()
    {
        // animIDSpeed = Animator.StringToHash("Speed");
        // animIDGrounded = Animator.StringToHash("Grounded");
        // animIDJump = Animator.StringToHash("Jump");
        // animIDFreeFall = Animator.StringToHash("FreeFall");
        // animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

}
