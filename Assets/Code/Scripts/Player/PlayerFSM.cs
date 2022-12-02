using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;
using FSM;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Events;

public class PlayerFSM : MonoBehaviour, IReset
{
    [field:SerializeField] public Animator animator { private set; get; }
    [field:SerializeField] public CharacterController characterController { private set; get; }

    [field:SerializeField] public Transform cameraTransform { private set; get; }
    [field:SerializeField] public float lerpRotationPct { private set; get; }

    [Header("Parkour Checkers")]
    [SerializeField] private Transform headWallChecker;
    [SerializeField] private Transform chestWallChecker;
    [SerializeField] private Transform kneesWallChecker;

    [SerializeField] private bool headWalling;
    [SerializeField] private bool chestWalling;
    [SerializeField] private bool kneesWalling;

    [SerializeField] private float wallDistance;

    [field:Space(10)]
    [field:Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [field:SerializeField] public float jumpTimeout { private set; get; } = 0.50f;
    
    [field:Tooltip("Useful for rough ground")]
    [field:SerializeField] public float groundedOffset { private set; get; } = -0.14f;
    
    [field:Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [field:SerializeField] public float groundedRadius { private set; get; } = 0.28f;
    
    [field:Tooltip("Useful for rough ground")]
    [field:SerializeField] public float headHitOffset { private set; get; } = -0.14f;
    
    [field:Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [field:SerializeField] public float headHitRadius { private set; get; } = 0.28f;
    
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
    
    [SerializeField] private float speed = 0.0f;
    
    [Header("Audios")]
    [SerializeField] private FMODUnity.EventReference playerStepEvent;
    
    // timeout deltatime
    private float _jumpTimeoutDelta;
    [field:SerializeField] public float _fallTimeoutDelta { private set; get; }

    public float _verticalVelocity;

    private StateMachine fsm;

    public Vector2 moveInput { private set; get; }
    private bool run;
    private Vector3 movement = Vector3.zero;
    
    public int animIDSpeed { private set; get; }
    public int animIDGrounded { private set; get; }
    public int animIDJump { private set; get; }
    public int animIDLongJump { private set; get; }
    public int animIDJumpCombo { private set; get; }
    public int animIDPunch { private set; get; }
    public int animIDPunchCombo { private set; get; }
    public int animIDCrouch { private set; get; }
    public int animIDFreeFall { private set; get; }
    public int animIDLand { private set; get; }
    public int animIDBumDrop { private set; get; }
    public int animIDPushWall { private set; get; }

    private bool jump;
    [field: SerializeField] public bool crouch { private set; get; }

    [SerializeField] private bool isAnalog = false;

    [HideInInspector] public int jumpCombo;
    public int punchCombo;

    [SerializeField] 
    private UnityEvent<string, string> OnStateChange;
    private string actualState = "none";
    private string lastState = "none";

    public bool pushWall = false;
    private Rigidbody pushWallObj;
    
    // Start is called before the first frame update
    void Start()
    {
        fsm = new StateMachine();
        
        AddStates();
        AddTransitions();
        
        fsm.SetStartState("Idle");
        fsm.Init();

        AssignAnimationIDs();
        GameManager.GetGameManager().SetPlayer(transform);

        _fallTimeoutDelta = fallTimeout;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (fsm.ActiveStateName != actualState)
        {
            lastState = actualState;
            actualState = fsm.ActiveStateName;
            OnStateChange?.Invoke(lastState, actualState);
        }
        GroundedCheck();
        GravityForce();
        WallCheckers();
        fsm.OnLogic();
    }

    private void AddStates()
    {
        fsm.AddState("Idle", new Idle(this));
        fsm.AddState("Walk", new Walk(this));
        fsm.AddState("Crouch", new Crouch(this));
        fsm.AddState("Jump01", new Jump01(this));
        fsm.AddState("Jump02", new Jump02(this));
        fsm.AddState("Jump03", new Jump03(this));
        fsm.AddState("LongJump", new LongJump(this));
        fsm.AddState("DoubleJump", new DoubleJump(this));
        fsm.AddState("Punch", new Punch(this));
        fsm.AddState("CrouchJump", new CrouchJump(this));
        fsm.AddState("Fall", new Fall(this));
        fsm.AddState("BumDrop", new BumDrop(this));
        fsm.AddState("Land", new Land(this));
        fsm.AddState("PushWall", new PushWall(this));
        fsm.AddState("WallJump", new WallJump(this));
    }

    private void AddTransitions()
    {
        fsm.AddTwoWayTransition("Idle", "Walk", t => moveInput != Vector2.zero);
        fsm.AddTwoWayTransition("Idle", "Crouch", t => crouch);
        fsm.AddTransitionFromAny("Fall",t => !grounded && characterController.velocity.y < 0 && _fallTimeoutDelta <= 0 && fsm.ActiveStateName != "BumDrop");
        fsm.AddTransition("Fall", "Land", t => grounded);
        fsm.AddTransition("BumDrop", "Land", t => grounded);
        fsm.AddTwoWayTransition("Walk", "PushWall", t => pushWall);
        fsm.AddTriggerTransitionFromAny(
            "Fall"
            ,new Transition("", "Fall", t => true));
        fsm.AddTriggerTransitionFromAny(
            "LongJump"
            ,new Transition("", "LongJump", t => fsm.ActiveStateName == "Walk"));
        fsm.AddTriggerTransitionFromAny(
            "Jump01"
            ,new Transition("", "Jump01", t => fsm.ActiveStateName == "Walk" || fsm.ActiveStateName == "Idle"));
        fsm.AddTriggerTransitionFromAny(
            "Jump02"
            ,new Transition("", "Jump02", t => fsm.ActiveStateName == "Land"));
        fsm.AddTriggerTransitionFromAny(
            "Jump03"
            ,new Transition("", "Jump03", t => fsm.ActiveStateName == "Land"));
        fsm.AddTriggerTransitionFromAny(
            "BumDrop"
            ,new Transition("", "BumDrop", t => true));
        fsm.AddTriggerTransitionFromAny(
            "CrouchJump"
            ,new Transition("", "CrouchJump", t => fsm.ActiveStateName == "Crouch"));
        fsm.AddTriggerTransitionFromAny(
            "Reset"
            ,new Transition("", "Idle", t => true));
        fsm.AddTriggerTransitionFromAny(
            "Punch"
            ,new Transition("", "Punch", t => fsm.ActiveStateName == "Walk" || fsm.ActiveStateName == "Idle"));
    }

    public void Move()
    {
        Vector3 forwardCamera = cameraTransform.forward;
        Vector3 rightCamera = cameraTransform.right;
        forwardCamera.y = 0;
        rightCamera.y = 0;
        Vector3 targetMovement = Vector3.zero;

        targetMovement = forwardCamera.normalized * moveInput.y + rightCamera.normalized * moveInput.x;
        targetMovement.y = 0.0f;
        movement = targetMovement;
        if (moveInput != Vector2.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetMovement);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, lerpRotationPct * Time.deltaTime);
        }

        switch (isAnalog)
        {
            case false when !run:
                animator.SetFloat(animIDSpeed, moveInput.magnitude/2);
                movement = movement / 2 * speed * Time.deltaTime;
                break;
            case false when run:
            case true:
                animator.SetFloat(animIDSpeed, moveInput.magnitude);
                movement = movement * speed * Time.deltaTime;
                break;
        }
        characterController.Move(movement +
                                    new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        characterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
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
    
    private void HeadHitCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - headHitOffset,
            transform.position.z);
        _verticalVelocity = Physics.CheckSphere(spherePosition, headHitRadius, groundLayers,
            QueryTriggerInteraction.Ignore) ? 0 : _verticalVelocity;
    }

    private void GravityForce()
    {
        if (grounded)
        {
            // reset the fall timeout timer
            if(_fallTimeoutDelta < fallTimeout){
                // Debug.Log("Grounded");
                _fallTimeoutDelta = fallTimeout;
            }

            if(_verticalVelocity < -1.0f) _verticalVelocity = -1.0f;
        }
        else
        {
            if(_verticalVelocity > 0) HeadHitCheck();
            
            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            _verticalVelocity += gravity * Time.deltaTime;
        }
        // jump timeout
        if (_jumpTimeoutDelta >= 0.0f)
        {
            _jumpTimeoutDelta -= Time.deltaTime;
        }
    }

    public void Jump(float JumpHeight)
    {
        _verticalVelocity = 0;
        
        // reset the jump timeout timer
        _jumpTimeoutDelta = jumpTimeout;

        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * gravity);
    }

    public void Reset()
    {
        fsm.Trigger("Reset");
    }
    
    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = isAnalog ? context.ReadValue<Vector2>() : context.ReadValue<Vector2>().normalized;
    }
    
    public void ReadRunInput(InputAction.CallbackContext context)
    {
        if (!run && context.performed)
        {
            run = true;
        }
        else if (context.canceled) run = false;
    }

    public void ReadCrouchInput(InputAction.CallbackContext context)
    {
        if (context.action.triggered && !crouch)
        {
            if(fsm.ActiveStateName == "Fall") fsm.Trigger("BumDrop");
        }
        if ((!crouch && !pushWall) && context.performed)
        {
            if (fsm.ActiveStateName == "Walk" && !headWalling && chestWalling && kneesWalling)
            {
                pushWall = true;
                return;
            }
            crouch = true;
        }
        else if (!context.performed && (crouch || pushWall))
        {
            pushWall = false;
            crouch = false;
            animator.SetBool(animIDCrouch, crouch);
        }
    }

    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        if (!context.action.triggered || _jumpTimeoutDelta >= 0) return;
        if(fsm.ActiveStateName == "Walk" && crouch)
        {
            fsm.Trigger("LongJump");
            return;
        }
        switch (jumpCombo)
        {
            case 0:
                fsm.Trigger("Jump01");
                break;
            case 1:
                fsm.Trigger("Jump02");
                break;
            case 2:
                fsm.Trigger("Jump03");
                break;
        }
        fsm.Trigger("CrouchJump");
    }
    
    public void ReadPunchInput(InputAction.CallbackContext context)
    {
        if (!context.action.triggered) return;
        switch (punchCombo)
        {
            case 0 when fsm.ActiveStateName == "Idle" || fsm.ActiveStateName == "Walk":
                punchCombo++;
                animator.SetInteger(animIDPunchCombo, punchCombo);
                fsm.Trigger("Punch");
                break;
            case 1 when fsm.ActiveStateName == "Punch":
            case 2 when (fsm.ActiveStateName == "Punch"):
                punchCombo++;
                animator.SetInteger(animIDPunchCombo, punchCombo);
                break;
        }
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDLongJump = Animator.StringToHash("LongJump");
        animIDJumpCombo = Animator.StringToHash("JumpCombo");
        animIDPunch = Animator.StringToHash("Punch");
        animIDPunchCombo = Animator.StringToHash("PunchCombo");
        animIDCrouch = Animator.StringToHash("Crouch");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDLand = Animator.StringToHash("Land");
        animIDBumDrop = Animator.StringToHash("BumDrop");
        animIDPushWall = Animator.StringToHash("PushWall");
    }

    public void OnDeviceChanged(PlayerInput playerInput)
    {
        foreach (var device in playerInput.devices)
        {
            isAnalog = device.GetType() != typeof(Keyboard) || device.GetType() != typeof(Mouse);
        }
    }

    private void CheckWallCollision()
    {
        RaycastHit headHit;
        headWalling = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out headHit,
            wallDistance, groundLayers);
    }

    public void PlayStepAudio(float velocity)
    {
        if(moveInput.magnitude > velocity) return;
        FMODUnity.RuntimeManager.PlayOneShot(playerStepEvent, transform.position);
    }
    
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        Gizmos.color = grounded ? transparentGreen : transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
        
        Gizmos.color = transparentGreen;
        
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - headHitOffset, transform.position.z),
            headHitRadius);
        var t = transform.position;
        t.y = 0;
        Gizmos.DrawRay(  t + new Vector3(0, headWallChecker.position.y, 0), transform.forward.normalized * wallDistance);
        Gizmos.DrawRay(t + new Vector3(0, chestWallChecker.position.y, 0), transform.forward.normalized * wallDistance);
        Gizmos.DrawRay(t + new Vector3(0, kneesWallChecker.position.y, 0), transform.forward.normalized * wallDistance);
    }

    public void FinishPunch(int punchNumb)
    {
        if(punchCombo <= punchNumb || animator.GetCurrentAnimatorStateInfo(0).IsName("Player.Idle/Walk/Run"))fsm.RequestStateChange(moveInput != Vector2.zero ? "Walk" : "Idle");
        // else animator.SetTrigger(animIDPunch);
    }

    public void FinishRecover()
    {
        jumpCombo = 0;
        animator.SetInteger(animIDJumpCombo, jumpCombo);
        fsm.RequestStateChange(moveInput != Vector2.zero ? "Walk" : "Idle");
    }

    private void WallCheckers()
    {
        headWalling = Physics.Raycast(headWallChecker.position, transform.forward, wallDistance, groundLayers);
        chestWalling = Physics.Raycast(chestWallChecker.position, transform.forward, wallDistance, groundLayers);
        kneesWalling = Physics.Raycast(kneesWallChecker.position, transform.forward, out var hitInfo, wallDistance, groundLayers);
        if (kneesWalling && hitInfo.transform.CompareTag("Box") && pushWallObj == null)
        {
            pushWallObj = hitInfo.transform.GetComponent<Rigidbody>();
        }
    }
}
