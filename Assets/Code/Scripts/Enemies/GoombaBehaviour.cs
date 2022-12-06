using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Unity.Mathematics;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GoombaBehaviour : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private string currentState;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform enemyEyes;
    [SerializeField] private Transform playerHead;
    private NavMeshPath path;
    [SerializeField] private List<Transform> m_PartolTargets;
    private int m_CurrentPatrolTargetId = 0;
    private int currentCornerId = 0;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rangeDistance;
    [SerializeField] private float sightDistance;
    [SerializeField] private float maxChaseDistance;
    [SerializeField] private float sightAngle;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private int maxHealth;
    [SerializeField] private float damage;
    [SerializeField] private GameObject[] dropObjects;
    
    private float health;
    private StateMachine fsm;
    private bool canBeDamaged = true;
    private string lastState;
    private int chaseAnimID;
    private int dieAnimID;
    private Vector3 resetPos;

    private void Awake()
    {
        chaseAnimID = Animator.StringToHash("Chase");
        dieAnimID = Animator.StringToHash("Death");
        resetPos = transform.position;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        
        fsm = new StateMachine();
        
        fsm.AddState("Idle", new State(
            onLogic: (state) => { fsm.RequestStateChange("Patrol"); }));
        fsm.AddState("Patrol", new State(
            onEnter: (state) =>
            {
                currentCornerId = 0;
                NavMesh.CalculatePath(transform.position, m_PartolTargets[m_CurrentPatrolTargetId].position,
                    NavMesh.AllAreas, path);
            },
            onLogic: (state) =>
            {
                if (state.timer.Elapsed >= 3.0f)
                {
                    NavMesh.CalculatePath(transform.position, m_PartolTargets[m_CurrentPatrolTargetId].position,
                        NavMesh.AllAreas, path);
                    currentCornerId = 0;
                    state.timer.Reset();
                }
 
                var aPos = transform.position;
                aPos.y = 0;
                var dPos = path.corners[currentCornerId];
                dPos.y = 0;
                
                var direction = dPos - aPos;
                Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
 
                characterController.Move(transform.forward.normalized * moveSpeed * Time.deltaTime 
                                            + new Vector3(0, -7, 0) * Time.deltaTime);
                
                if(PathCornerPositionArrived())
                    MoveToNextCornerPosition();
                
                if (PatrolTargetPositionArrived())
                    MoveToNextPatrolPosition();
            },
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
            }
            ));
        fsm.AddState("Chase", new State(
            onEnter: (state) =>
            {
                currentCornerId = 0;
                NavMesh.CalculatePath(transform.position, GameManager.GetGameManager().GetPlayer().transform.position,
                NavMesh.AllAreas, path);
            },
            onLogic: (state) =>
            {
                if (state.timer.Elapsed >= 1.0f)
                {
                    NavMesh.CalculatePath(transform.position,
                        GameManager.GetGameManager().GetPlayer().transform.position,
                        NavMesh.AllAreas, path);
                    currentCornerId = 0;
                    state.timer.Reset();
                }
                
                for (int i = 0; i < path.corners.Length - 1; i++)
                    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

                var aPos = transform.position;
                aPos.y = 0;
                if (currentCornerId >= path.corners.Length) currentCornerId = 0;
                var dPos = path.corners[currentCornerId];
                dPos.y = 0;
                
                var direction = dPos - aPos;
                Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
 
                characterController.Move(transform.forward.normalized * moveSpeed * 2 * Time.deltaTime 
                                         + new Vector3(0, -7, 0) * Time.deltaTime);
                
                if(PathCornerPositionArrived())
                    MoveToNextCornerPosition();
            },
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
            }
            ));
        fsm.AddState("Hit", new State(
            onEnter: (state) =>
            {
                StartCoroutine(Hit());
                canBeDamaged = false;
            },
            onLogic: (state) =>
            {
                if (state.timer.Elapsed >= 2.0f)
                {
                    fsm.RequestStateChange(lastState == "Patrol" ? "Alert" : lastState);
                }
            },
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
                StartCoroutine(CanBeAttacked());
            }
            ));
        fsm.AddState("Die", new CoState(
            this,
            onEnter: (state) =>
            {
                canBeDamaged = false;
            },
            onLogic: Die
            ));
        
        fsm.AddTransition(new Transition(
            "Patrol",
            "Chase",
            (transition) => SeesPlayer() && PlayerOnRange()
        ));
        fsm.AddTransition(new Transition(
            "Chase",
            "Patrol",
            (transition) => !CanChase()
        ));
        // fsm.AddTriggerTransitionFromAny(
        //     "OnHit",
        //     new Transition("", "Hit", t => (health > 0))
        // );
        // fsm.AddTransitionFromAny(
        //     new Transition("", "Die", t => (health <= 0))
        // );
        // fsm.AddTriggerTransitionFromAny(
        //     "Reset"
        //     ,new Transition("", "Idle", t => true));
        
        fsm.SetStartState("Idle");
        fsm.Init();

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        playerHead = Camera.main.transform;
        fsm.OnLogic();
        if(fsm.ActiveState.name == "Attack")
        {
            transform.rotation = Quaternion.Euler(0.0f, enemyEyes.rotation.eulerAngles.y, 0.0f);
            enemyEyes.LookAt(playerHead);
        }
        currentState = fsm.ActiveState.name;
    }
    
    private bool PathCornerPositionArrived()
    {
        return Vector3.Distance(transform.position, path.corners[currentCornerId]) < 0.3;
    }
    
    private void MoveToNextCornerPosition()
    {
        ++currentCornerId;
        if (currentCornerId < path.corners.Length) return;
        currentCornerId = 0;
        if(fsm.ActiveStateName == "Patrol") MoveToNextPatrolPosition();
    }
    
    private bool PatrolTargetPositionArrived()
    {
        return Vector3.Distance(transform.position, m_PartolTargets[m_CurrentPatrolTargetId].position) < 0.3;
    }
    
    private void MoveToNextPatrolPosition()
    {
        ++m_CurrentPatrolTargetId;
        if (m_CurrentPatrolTargetId >= m_PartolTargets.Count)
            m_CurrentPatrolTargetId = 0;
        NavMesh.CalculatePath(transform.position, m_PartolTargets[m_CurrentPatrolTargetId].position,
            NavMesh.AllAreas, path);
    }

    bool CanChase()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        return Vector3.Distance(PlayerPos, transform.position) <= maxChaseDistance;
    }
    
    bool PlayerOnRange()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        return Vector3.Distance(PlayerPos, transform.position) <= rangeDistance;
    }
    
    bool SeesPlayer()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        Vector3 l_DirectionPlayerXZ = PlayerPos - transform.position;
        l_DirectionPlayerXZ.Normalize();
        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.Normalize();

        Vector3 l_Direction = playerHead.position - enemyEyes.position;
        float l_Lenght = l_Direction.magnitude;
        l_Direction /= l_Lenght;

        Ray l_Ray = new Ray(enemyEyes.position, l_Direction);
        return Vector3.Distance(PlayerPos, transform.position) < sightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionPlayerXZ) > Mathf.Cos(sightAngle * Mathf.Deg2Rad / 2.0f) &&
               !Physics.Raycast(l_Ray, l_Lenght, playerLayerMask.value);
    }
    
    private void RotateAtSpeed(float speed)
    {
        transform.Rotate(Vector3.up, rotationSpeed*Time.deltaTime);
    }
    
    private void MoveTowardsPlayer(float speed)
    {
        // m_NavMeshAgent.destination = playerHead.transform.position;
    }

    public void TakeDamage(float damage)
    {
        if (canBeDamaged)
        {
            fsm.Trigger("OnHit");
            health -= damage;
        }
        else
        {
            health -= damage * 0.6f;
        }
    }

    private IEnumerator Hit()
    {
        yield return new WaitForSeconds(1f);
    }
    
    private IEnumerator Die(CoState<string, string> state)
    {
        animator.SetTrigger(dieAnimID);

        yield return new WaitForSeconds(1f);
        

        yield return new WaitForSeconds(0.5f);
        
        state.timer.Reset();
        while (state.timer.Elapsed < 1f)
        {
            enemyEyes.transform.position += Vector3.down*Time.deltaTime*.70f/1f;
            yield return null;
        }

        Instantiate(dropObjects[Random.Range(0, dropObjects.Length)], transform.position, quaternion.identity);
        yield return new WaitForSeconds(3f);
        
        gameObject.SetActive(false);
    }

    private IEnumerator CanBeAttacked()
    {
        yield return new WaitForSeconds(1.5f);
        canBeDamaged = true;
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        transform.position = resetPos;
        fsm.Trigger("Reset");
        StopAllCoroutines();
        health = maxHealth;
    }
}