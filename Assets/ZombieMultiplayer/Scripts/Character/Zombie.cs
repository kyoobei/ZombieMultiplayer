using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Zombie : CharacterBase
{
    NavMeshAgent zombieAgent;
    GameObject targetObject;
    ZombieDetection zombieDetection;
    ZombieEater zombieEater;
    enum ZombieAIBehaviour
    {
        STATIONARY,
        PATROLLING
    };
    [SerializeField] private ZombieAIBehaviour initialZombieBehavior;
    [SerializeField] private ZombieAIBehaviour currentZombieBehavior;
    [SerializeField] private float patrolDistance;
    [SerializeField] private bool isEating;

    Vector3 initialPosition;
    bool hasGotInitialPosition;
    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        zombieDetection = GetComponentInChildren<ZombieDetection>();
        zombieEater = GetComponentInChildren<ZombieEater>();

        zombieAgent.speed = moveSpeed;
        zombieAgent.angularSpeed = rotateSpeed;

        currentZombieBehavior = GetRandomBehaviour();
        initialZombieBehavior = currentZombieBehavior;
    }
    private void Update()
    {
        if (!hasGotInitialPosition)
        {
            if(zombieAgent.isOnNavMesh)
            {
                initialPosition = transform.position;
                hasGotInitialPosition = true;
            }
        }
        else
        {
            UpdateCharacterState();
        }
    }
    #region OVERRIDES
    protected override void IdleState()
    {
        base.IdleState();
        switch(currentZombieBehavior)
        {
            case ZombieAIBehaviour.STATIONARY:
                //add code here to change from stationary to 
                //patrolling
                break;
            case ZombieAIBehaviour.PATROLLING:
                characterState = CharacterState.MOVING;
                break;
        }
    }
    protected override void MovingState()
    {
     
    }
    protected override void SpecialAction()
    {
        base.SpecialAction();
        //for eating sequence;
        //if done with the special action go back to the initial
        //AI type
    }
    #endregion
    private ZombieAIBehaviour GetRandomBehaviour()
    {
        int randomVal = Random.Range(1, 15);
        if (randomVal % 2 == 0)
            return ZombieAIBehaviour.STATIONARY;
        else
            return ZombieAIBehaviour.PATROLLING;
    }
    /*
    [SerializeField] ZombieDetection zombieDetection;
    [SerializeField] ZombieEater zombieEater;
    [SerializeField] GameObject target;
    [SerializeField] Animator zombieAnimator;
    NavMeshAgent zombieAgent;

    public float moveSpeed;
    public float multiplierSpeed;

    private Vector3 initialPosSummoned;
    bool getInitialPos;
    public enum ZombieType
    {
        STATIONARY,
        PATROLLING
    };
    public enum ZombieState
    {
        IDLE,
        CHASING,
        EATING
    };
    public ZombieState zombieState;
    public ZombieType zombieType;

    Rigidbody zombieRigidbody;
    CharacterController characterController;
    bool isEating;
    private void Start()
    {
        zombieRigidbody = GetComponent<Rigidbody>();
        zombieAgent = GetComponent<NavMeshAgent>();
        //characterController = GetComponent<CharacterController>();
        
    }
    private void Update()
    {
        UpdateState();
    }
    void UpdateState()
    {
        switch(zombieState)
        {
            case ZombieState.IDLE:
                
                if(zombieDetection.hasDetectedAPlayer)
                {
                    zombieState = ZombieState.CHASING;
                    target = zombieDetection.target;
                }
                else
                {
                    MovePersonally();
                    target = null; 
                }
                isEating = false;
                break;
            case ZombieState.CHASING:
                if(!zombieDetection.hasDetectedAPlayer)
                {
                    
                    zombieState = ZombieState.IDLE;
                }
                MoveTowardsPlayer();
                if (zombieEater.isEating)
                {
                    target = zombieEater.targetEaten;
                    zombieState = ZombieState.EATING;
                }
                break;
            case ZombieState.EATING:
                UpdateEating();
                //perform animation of eating then if eaten
                //then back to idle state
                break;
        }
    }
    void MovePersonally()
    {
        if(!getInitialPos)
        {
            initialPosSummoned = transform.position;
            getInitialPos = true;
        }
        float distance = Vector3.Distance(initialPosSummoned, transform.position);
        if(distance <= 0.3f)
        {
            zombieAnimator.SetFloat("movement", 0f);
        }
        else
        {
            zombieAgent.SetDestination(initialPosSummoned);
            zombieAnimator.SetFloat("movement", 0.2f);
        }
    }
    void MoveTowardsPlayer()
    {
        if(target != null)
        {
            transform.LookAt(
                new Vector3
                (
                    target.transform.position.x, 
                    transform.position.y,
                    target.transform.position.z
                ));
           
            zombieAgent.SetDestination(target.transform.position);
            zombieAnimator.SetFloat("movement", 0.2f);
        }
    }
    private void UpdateEating()
    {
        if(!isEating)
        {
            if (target != null)
            {
                zombieAnimator.SetBool("isAttacking", true);
                //Player playerDetected = target.GetComponent<Player>();
                //playerDetected.InitiateBeingEaten();
                isEating = true;
            }
        }
        if(zombieEater.targetEaten == null)
        {
            zombieAnimator.SetBool("isAttacking", false);
            isEating = false;
            zombieState = ZombieState.IDLE;
        }
    }
    void UpdateIdleStateOfZombie()
    {
        switch(zombieType)
        {
            case ZombieType.STATIONARY:
                break;
            case ZombieType.PATROLLING:
                break;
        }
    }
    */
}
