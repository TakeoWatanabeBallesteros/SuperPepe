using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FSM;

public class PlayerFSM : MonoBehaviour, IReset
{
    [Header("Inputs")] 
    [SerializeField] 
    public InputActionReference moveInput;
    [SerializeField] 
    private InputActionReference jumpInput;
    [SerializeField] 
    private InputActionReference runInput;
    [SerializeField] 
    private InputActionReference crouchInput;
    [SerializeField] 
    private InputActionReference tpInput;

    [Space] 
    [SerializeField] 
    public float walkSpeed;
    [SerializeField] 
    public float runSpeed;
    [SerializeField] 
    public float crouchSpeed;
    [SerializeField] 
    public float gravity;
    [SerializeField] 
    public float jumpHeight;
    [SerializeField] 
    public float crouchHeight;
    [Space] 
    [SerializeField] 
    public Transform crouchPivot;
    
    public bool grounded => controller.isGrounded;
    public Vector2 MoveInput => moveInput.action.ReadValue<Vector2>().normalized;
    [HideInInspector]
    public float currentSpeed;

    private StateMachine fsm;
    [HideInInspector]
    public CollisionFlags collisionFlags;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public float verticalVelocity;

    private Vector3 tpPosition;

    public static bool canShoot{get; private set;}
    public static bool canThrow{get; private set;}

    private void OnEnable()
    {
        moveInput.action.Enable();
        runInput.action.Enable();
        jumpInput.action.Enable();
        crouchInput.action.Enable();
#if UNITY_EDITOR
        tpInput.action.Enable();
        tpInput.action.performed += _ =>
        {
            transform.position = tpPosition;
        };
#endif
    }

    private void OnDisable()
    {
        moveInput.action.Disable();
        runInput.action.Disable();
        jumpInput.action.Disable();
        crouchInput.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // tpPosition = transform.position;
        fsm = new StateMachine();
        controller = GetComponent<CharacterController>();
        
        AddStates();
        AddTransitions();
        
        fsm.SetStartState("Idle");
        fsm.Init();

        canShoot = true;
        canThrow = !canShoot;
        
        GameManager.GetGameManager().SetPlayer(transform);
    }
    // Update is called once per frame
    void Update()
    {
        fsm.OnLogic();
        
        // Always keep "pushing it" to maintain contact
        if (controller.isGrounded)  
            verticalVelocity = gravity;
        // Accelerate
        else
            verticalVelocity += gravity * Time.deltaTime;
    }

    private void AddStates()
    {
        fsm.AddState("Idle", new Idle(this));
        fsm.AddState("Walk", new Walk(this));
        fsm.AddState("Crouch", new Crouch(this));
        fsm.AddState("Jump", new Jump(this));
        fsm.AddState("Fall", new Fall(this));
        fsm.AddState("Land", new Land(this));
    }

    private void AddTransitions()
    {
        fsm.AddTwoWayTransition("Idle", "Walk", t => moveInput.action.ReadValue<Vector2>() != Vector2.zero && crouchInput.action.ReadValue<float>() == 0);
        fsm.AddTwoWayTransition("Idle", "Crouch", t => crouchInput.action.ReadValue<float>() > 0);
        fsm.AddTwoWayTransition("Walk", "Crouch", t => crouchInput.action.ReadValue<float>() > 0);
        fsm.AddTransition("Fall", "Land", t => grounded);
        fsm.AddTransitionFromAny(new Transition("", "Jump", t => jumpInput.action.triggered && grounded));
        fsm.AddTransitionFromAny(new Transition("", "Fall", t => verticalVelocity <= 0 && !grounded));
        fsm.AddTriggerTransitionFromAny(
            "Reset"
            ,new Transition("", "Idle", t => true));
    }

    public static void ChangeShoot() => (canShoot, canThrow) = (canThrow, canShoot);
    public void Reset()
    {
        fsm.Trigger("Reset");
    }
}
