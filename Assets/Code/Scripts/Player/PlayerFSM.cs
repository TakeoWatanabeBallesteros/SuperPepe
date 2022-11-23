using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FSM;
using UnityEngine.InputSystem.XInput;

public class PlayerFSM : MonoBehaviour, IReset
{
    [field:SerializeField] public Animator animator { private set; get; }
    [field:SerializeField] public Rigidbody rb { private set; get; }

    [field:SerializeField] public Transform cameraTransform { private set; get; }
    [field:SerializeField] public float lerpRotationPct { private set; get; }
    
    [field:SerializeField] public float walkSpeed { private set; get; }
    
    [field:SerializeField] public float runSpeed { private set; get; }

    
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
    [field:SerializeField] public float jump01Height { private set; get; } = 1.2f;
    
    [field:Tooltip("The height the player can jump")]
    [field:SerializeField] public float jump02Height { private set; get; } = 1.2f;
    
    [field:Tooltip("The height the player can jump")]
    [field:SerializeField] public float jump03Height { private set; get; } = 1.2f;
    
    [field:Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [field:SerializeField] public float gravity { private set; get; } = -15.0f;
    
    [Header("Audios")]
    [SerializeField] private FMODUnity.EventReference playerStepEvent;
    
    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    public float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private StateMachine fsm;

    private Vector2 moveInput;
    private Vector3 movement;
    
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDLand;
    private int animIDCrouch;
    private int animIDFreeFall;

    private bool jump;
    private bool crouch;

    private bool isAnalog = true;
    
    // Start is called before the first frame update
    void Start()
    {
        fsm = new StateMachine();
        
        AddStates();
        // AddTransitions();
        
        fsm.SetStartState("Idle");
        fsm.Init();

        AssignAnimationIDs();
        // GameManager.GetGameManager().SetPlayer(transform);
    }
    
    // Update is called once per frame
    void Update()
    {
        GroundedCheck();
        GravityForce();
        fsm.OnLogic();
        animator.SetBool(animIDCrouch, crouch);
    }

    private void AddStates()
    {
        fsm.AddState("Idle", new Idle(this));
        fsm.AddState("Walk", new Walk(this));
        fsm.AddState("Crouch", new Crouch(this));
        fsm.AddState("Jump", new Jump(this));
        fsm.AddState("DoubleJump", new DoubleJump(this));
        fsm.AddState("TripleJump", new TripleJump(this));
        fsm.AddState("Fall", new Fall(this));
        fsm.AddState("Land", new Land(this));
    }

    private void AddTransitions()
    {
        fsm.AddTwoWayTransition("Idle", "Walk", t => moveInput != Vector2.zero);
        fsm.AddTwoWayTransition("Idle", "Crouch", t => crouch);
        fsm.AddTwoWayTransition("Walk", "Crouch", t => crouch);
        fsm.AddTransition("Fall", "Land", t => grounded);
        fsm.AddTransitionFromAny(new Transition("", "Fall", t => _verticalVelocity <= 0 && !grounded));
        fsm.AddTriggerTransitionFromAny(
            "Jump"
            ,new Transition("", "Jump", t => grounded));
        fsm.AddTriggerTransitionFromAny(
            "Reset"
            ,new Transition("", "Idle", t => true));
    }

    public void Move()
    {
        float speed = 0.0f;

        Vector3 forwardCamera = cameraTransform.forward.normalized;
        Vector3 rightCamera = cameraTransform.right.normalized;

        Vector3 targetMovement = Vector3.zero;

        targetMovement = forwardCamera * moveInput.y + rightCamera * moveInput.x;
        targetMovement.y = 0.0f;
        movement = Vector3.Slerp(movement, targetMovement, lerpRotationPct * Time.deltaTime);

        if (moveInput != Vector2.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetMovement);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, lerpRotationPct * Time.deltaTime);
        }

        animator.SetFloat(animIDSpeed, movement.magnitude);
        movement = movement * 1600 * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
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
        animator.SetBool(animIDGrounded, grounded);
    }

    private void GravityForce()
    {
        if (grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = fallTimeout;

            // update animator if using character
            animator.SetBool(animIDFreeFall, false);

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
            
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
                animator.SetBool(animIDFreeFall, true);
            }
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public void Jump(float JumpHeight)
    {
        if (!(_jumpTimeoutDelta <= 0.0f)) return;
        // Jump
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * gravity);

        // update animator if using character
        animator.SetTrigger(animIDJump);
    }

    public void Reset()
    {
        fsm.Trigger("Reset");
    }
    
    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = isAnalog ? context.ReadValue<Vector2>().normalized : context.ReadValue<Vector2>();
    }

    public void ReadCrouchInput(InputAction.CallbackContext context)
    {
        if (!crouch && context.performed)
        {
            crouch = true;
        }
        else if (context.canceled) crouch = false;
    }
    
    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        fsm.Trigger("Jump");
    }
    
    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDLand = Animator.StringToHash("Land");
        animIDCrouch = Animator.StringToHash("Crouch");
        animIDFreeFall = Animator.StringToHash("FreeFall");
    }

    public void OnDeviceChanged(PlayerInput playerInput)
    {
        foreach (var device in playerInput.devices)
        {
            isAnalog = device.GetType() == typeof(Keyboard);
        }
    }

    public void PlayStepAudio(float velocity)
    {
        if(moveInput.magnitude > velocity) return;
        FMODUnity.RuntimeManager.PlayOneShot(playerStepEvent, transform.position);
    }
}
