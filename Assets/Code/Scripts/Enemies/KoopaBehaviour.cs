using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Pathfinding;

public class KoopaBehaviour : MonoBehaviour
{
    private IAstarAI agent;
    [SerializeField] private string currentState;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform enemyEyes;
    [SerializeField] private Transform playerHead;
    [SerializeField] private List<Transform> patrolTargets;
    private int currentPatrolTargetId = 0;
    [SerializeField] private float rangeDistance;
    [SerializeField] private float sightDistance;
    [SerializeField] private float maxChaseDistance;
    [SerializeField] private float sightAngle;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private int maxHealth;
    [SerializeField] private float damage;
    
    private float health;
    private StateMachine fsm;
    private bool canBeDamaged = true;
    private string lastState;
    private int chaseAnimID;
    private int dieAnimID;
    private Vector3 resetPos;

    private void Awake()
    {
        agent = GetComponent<IAstarAI>();
        chaseAnimID = Animator.StringToHash("Chase");
        dieAnimID = Animator.StringToHash("Death");
        resetPos = transform.position;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        fsm = new StateMachine();
        
        fsm.AddState("Idle", new State(
            onLogic: (state) => { fsm.RequestStateChange("Patrol"); }));
        fsm.AddState("Patrol", new State(
            onEnter: (state) =>
            {
                agent.maxSpeed = 1.7f;
            },
            onLogic: (state) =>
            {
                bool search = false;
               
                if (agent.reachedEndOfPath && !agent.pathPending) {
                    currentPatrolTargetId++;
                    search = true;
                }

                currentPatrolTargetId %= patrolTargets.Count;
                agent.destination = patrolTargets[currentPatrolTargetId].position;

                if (search) agent.SearchPath();
            },
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
            }
            ));
        fsm.AddState("Chase", new State(
            onEnter: (state) =>
            {
                agent.maxSpeed = 3;
                animator.SetBool(chaseAnimID, true);
            },
            onLogic: (state) =>
            {
               if(state.timer.Elapsed < .3) return;
               
               agent.destination = GameManager.GetGameManager().GetPlayer().transform.position;
            },
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
                animator.SetBool(chaseAnimID, false);
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
        fsm.OnLogic();
        currentState = fsm.ActiveState.name;
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

    public void TakeDamage(float damage)
    {
        if (canBeDamaged)
        {
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