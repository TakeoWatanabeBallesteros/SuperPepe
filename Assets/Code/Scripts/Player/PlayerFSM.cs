using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FSM;

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
    [field:SerializeField] public float jumpHeight { private set; get; } = 1.2f;
    [field:Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [field:SerializeField] public float gravity { private set; get; } = -15.0f;
    
    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    public float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private StateMachine fsm;

    private Vector2 moveInput;
    
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int nimIDFreeFall;
    private int animIDMotionSpeed;

    private bool jump;
    private bool crouch;
    

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
        fsm.OnLogic();
        if (!jump) return;
        jump = false;
        Debug.Log("jump");
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
        fsm.AddTransitionFromAny(new Transition("", "Jump", t => jump && grounded));
        fsm.AddTransitionFromAny(new Transition("", "Fall", t => _verticalVelocity <= 0 && !grounded));
        fsm.AddTriggerTransitionFromAny(
            "Reset"
            ,new Transition("", "Idle", t => true));
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

    public void Reset()
    {
        // fsm.Trigger("Reset");
    }
    
    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void ReadCrouchInput(InputAction.CallbackContext context)
    {
        crouch = !(context.ReadValue<float>() < 1);
    }
    
    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        jump = context.action.triggered;
    }
    
    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        nimIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}
