using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [field:SerializeField] public Animator animator { private set; get; }
    [field:SerializeField] public Rigidbody rb { private set; get; }

    [field:SerializeField] public Transform cameraTransform { private set; get; }
    [field:SerializeField] public float lerpRotationPct { private set; get; }
    [field:SerializeField] public float walkSpeed { private set; get; }
    [field:SerializeField] public float runSpeed { private set; get; }

    private Vector2 moveInput;
    private int animSpeedID;
    
    [field:Space(10)]
    [field:Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [field:SerializeField] public float jumpTimeout { private set; get; } = 0.50f;
    [field:Tooltip("Useful for rough ground")]
    [field:SerializeField] public float groundedOffset { private set; get; } = -0.14f;
    [field:Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [field:SerializeField] public float groundedRadius { private set; get; } = 0.28f;
    [field:Tooltip("What layers the character uses as ground")]
    [field:SerializeField] public LayerMask groundLayers { private set; get; }
    [field:Header("Player Grounded")]
    [field:Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [field:SerializeField] public bool grounded { private set; get; } = true;
    [field:Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [field:SerializeField] public float fallTimeout { private set; get; } = 0.15f;
    
    [field:Space(10)]
    [field:Tooltip("The height the player can jump")]
    [field:SerializeField] public float jumpHeight { private set; get; } = 1.2f;
    [field:Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [field:SerializeField] public float gravity { private set; get; } = -15.0f;
    
    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;


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
        movement = movement.normalized;

        float movementSpeed = 0.0f;
        if (moveInput != Vector2.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, lerpRotationPct * Time.deltaTime);
        }

        animator.SetFloat(animSpeedID, movement.magnitude);
        movement = movement * 800 * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
        rb.velocity = movement;
    }
    
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        // animator.SetBool(animIDGrounded, grounded); 
            
    }

    private void JumpAndGravity()
    {
        if (grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = fallTimeout;
            // update animator if using character
            // animator.SetBool(animIDJump, false);
            // animator.SetBool(animIDFreeFall, false);
 
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
 
            // Jump01
            // if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            // {
            //     // the square root of H * -2 * G = how much velocity needed to reach desired height
            //     _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //     // update animator if using character
            //     // animator.SetBool(animIDJump, true);
            //     
            // }
 
            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = jumpTimeout;
            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                // animator.SetBool(animIDFreeFall, true);
            }
 
            // if we are not grounded, do not jump
            // _input.jump = false;
        }
 
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void AssignAnimationIDs()
    {
        // animIDSpeed = Animator.StringToHash("Speed");
        // animIDGrounded = Animator.StringToHash("Grounded");
        // animIDJump = Animator.StringToHash("Jump01");
        // animIDFreeFall = Animator.StringToHash("FreeFall");
        // animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

}
