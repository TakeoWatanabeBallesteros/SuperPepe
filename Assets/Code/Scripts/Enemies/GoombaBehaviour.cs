using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Pathfinding;

public class GoombaBehaviour : MonoBehaviour, IReset
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
    private int chaseAnimID;
    private int dieAnimID;
    private Vector3 resetPos;

    private void Awake()
    {
        agent = GetComponent<IAstarAI>();
        chaseAnimID = Animator.StringToHash("Chase");
        dieAnimID = Animator.StringToHash("Death");
        resetPos = transform.position;
        GameManager.GetGameManager().AddResetObject(this);
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
                animator.SetBool(chaseAnimID, false);
            }
            ));
        fsm.AddState("Hit", new State(
            onEnter: (state) =>
            {
                agent.isStopped = true;
                canBeDamaged = false;
            },
            onLogic: (state) =>
            { 
                transform.position += -transform.forward.normalized * 3 * Time.deltaTime;
                if(state.timer.Elapsed > .4f) fsm.RequestStateChange("Chase");
            },
            onExit: (state) =>
            {
                agent.isStopped = false;
                canBeDamaged = true;
            }
        ));
        fsm.AddState("Die", new CoState(
            this,
            onEnter: (state) =>
            {
                agent.isStopped = true;
                canBeDamaged = false;
                animator.SetTrigger(dieAnimID);
            },
            onLogic: Die,
            
            onExit: (state) =>
            {
                
            }
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
        
        fsm.AddTriggerTransition(
            "Hit"
            ,new Transition("Chase", "Hit", t => true));
        fsm.AddTriggerTransitionFromAny(
            "Die"
            , new Transition("", "Die", t => true)
        );
        fsm.AddTriggerTransitionFromAny(
            "Reset"
            ,new Transition("", "Idle", t => true));
        
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
    
    private IEnumerator Die(CoState<string, string> state)
    {
        animator.SetTrigger(dieAnimID);

        yield return new WaitForSeconds(.2f);
        gameObject.SetActive(false);
    }

    private IEnumerator CanBeAttacked()
    {
        yield return new WaitForSeconds(1.5f);
        canBeDamaged = true;
    }

    public void Reset()
    {
        if (fsm.ActiveStateName == "Die")
        {
            GameManager.GetGameManager().RemoveResetObject(this);
            Destroy(gameObject);
        }
        gameObject.SetActive(true);
        transform.position = resetPos;
        fsm.Trigger("Reset");
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player") || !canBeDamaged) return;
        if(other.transform.position.y - transform.position.y < .04f){
            fsm.Trigger("Hit");
            other.GetComponent<PlayerFSM>().Hit(transform.forward);
        }
        else
        {
            fsm.Trigger("Die");
            other.GetComponent<PlayerFSM>().Jump(2);
        }
    }
}