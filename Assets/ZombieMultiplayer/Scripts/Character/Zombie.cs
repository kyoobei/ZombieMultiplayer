using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Zombie : CharacterBase
{
    public List<GameObject> listOfPatrolPoints = new List<GameObject>();
    [SerializeField] List<int> patrolledIndexesList = new List<int>();
    NavMeshAgent zombieAgent;
    GameObject targetObject;
    ZombieDetection zombieDetection;
    ZombieEater zombieEater;
    enum ZombieAIBehaviour
    {
        STATIONARY,
        PATROLLING,
        DETECTED
    };
    [SerializeField] private ZombieAIBehaviour initialZombieBehavior;
    [SerializeField] private ZombieAIBehaviour currentZombieBehavior;
    [SerializeField] private bool isEating;
    [SerializeField] private float maxWaitTime;
    
    float m_currentWaitTime;
    Vector3 m_targetPatrolPosition;
    bool m_isPatrolPosAcquired;
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
        //disabled for now to force patrolling state
        currentZombieBehavior = GetRandomBehaviour();
        initialZombieBehavior = currentZombieBehavior;

        patrolledIndexesList.Clear();
    }
    private void Update()
    {
        //the agent is grounded
        if(zombieAgent.isOnNavMesh)
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
                m_isPatrolPosAcquired = false;
                if (zombieDetection.hasDetectedAPlayer)
                {
                    currentZombieBehavior = ZombieAIBehaviour.DETECTED;
                }
                else if(!zombieDetection.hasDetectedAPlayer)
                {
                    zombieAgent.isStopped = true;
                    if(m_currentWaitTime < maxWaitTime)
                    {
                        m_currentWaitTime += Time.deltaTime;
                    }
                    else if(m_currentWaitTime >= maxWaitTime)
                    {
                        m_currentWaitTime = 0;
                        currentZombieBehavior = GetRandomBehaviour();
                    }
                }
                break;
            case ZombieAIBehaviour.PATROLLING:
                characterState = CharacterState.MOVING;
                break;
            case ZombieAIBehaviour.DETECTED:
                characterState = CharacterState.MOVING;
                break;
        }
    }
    protected override void MovingState()
    {
        if(currentZombieBehavior.Equals(ZombieAIBehaviour.DETECTED))
        { 
            zombieAgent.speed = moveSpeed;
            //has detected a player
            if (targetObject != null)
            {
                //chase player
                isEating = zombieEater.isEating;
                if(!isEating)
                {
                    zombieAgent.isStopped = false;
                    zombieAgent.SetDestination(targetObject.transform.position);
                }
                else
                {
                    zombieAgent.isStopped = true;
                    characterState = CharacterState.SPECIALACTION;
                }

            }
            else if(targetObject == null)
            {
                //player is gone (either disconnected or turned already)
                //go back to initial position
                //reset state to what it was the first time
                currentZombieBehavior = GetRandomBehaviour();
                if (currentZombieBehavior.Equals(ZombieAIBehaviour.STATIONARY))
                    characterState = CharacterState.IDLE;
                else
                {
                    m_isPatrolPosAcquired = false;
                }
            }
        }
        else if (currentZombieBehavior.Equals(ZombieAIBehaviour.PATROLLING))
        {
            if (zombieDetection.hasDetectedAPlayer)
            {
                currentZombieBehavior = ZombieAIBehaviour.DETECTED;
                m_isPatrolPosAcquired = false;
            }
            else if (!zombieDetection.hasDetectedAPlayer)
            {
                
                if (!m_isPatrolPosAcquired)
                {
                    //patrol logic here
                    zombieAgent.speed = (moveSpeed / 2f); //half move speed so that it will not be fast
                    zombieAgent.isStopped = false;
                    int randomIndex = GetRandomPatrolPoint();
                    if (!patrolledIndexesList.Contains(randomIndex))
                    {
                        patrolledIndexesList.Add(randomIndex);
                    }
                    else
                    {
                        //if it already contains the index
                        //contained equals amount of patrols count
                        if ((patrolledIndexesList.Count).Equals(listOfPatrolPoints.Count))
                        {
                            patrolledIndexesList.Clear();
                            randomIndex = GetRandomPatrolPoint();
                            patrolledIndexesList.Add(randomIndex);
                        }
                        else
                        {
                            List<int> checkIndexList = new List<int>(patrolledIndexesList);
                            checkIndexList.Sort();
                            for (int i = 0; i < checkIndexList.Count; i++)
                            {
                                if (i != checkIndexList[i])
                                {
                                    randomIndex = i;
                                    patrolledIndexesList.Add(i);
                                    return;
                                }
                            }
                        }
                    }
                    m_targetPatrolPosition = listOfPatrolPoints[randomIndex].transform.position;
                    zombieAgent.SetDestination(m_targetPatrolPosition);
                    m_isPatrolPosAcquired = true;
                }
                else
                {
                    if(zombieAgent.remainingDistance <= 0.1f)
                    {
                        currentZombieBehavior = GetRandomBehaviour();
                        
                        if (currentZombieBehavior.Equals(ZombieAIBehaviour.STATIONARY))
                            characterState = CharacterState.IDLE;
                        
                        zombieAgent.isStopped = true;
                        m_isPatrolPosAcquired = false;
                    }
                }
            }
        }
    }
    protected override void SpecialAction()
    {
        base.SpecialAction();
        if(targetObject == null)
        {
            //the player has been eaten already or possible disconnection
            //get back to random state that the zombie want to do
            currentZombieBehavior = GetRandomBehaviour();
            if (currentZombieBehavior.Equals(ZombieAIBehaviour.STATIONARY)) 
            {
                characterState = CharacterState.IDLE;
                m_currentWaitTime = 0;
                m_isPatrolPosAcquired = false;
            }
            else if (currentZombieBehavior.Equals(ZombieAIBehaviour.PATROLLING))
            {
                characterState = CharacterState.MOVING;
                m_currentWaitTime = 0;
                m_isPatrolPosAcquired = false;
            }
        }
        else
        {
            // to look at whatever player it is
            transform.LookAt(targetObject.transform);
        }
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
    private int GetRandomPatrolPoint()
    {
        int randomVal = Random.Range(0, listOfPatrolPoints.Count - 1);
        return randomVal;
    }
    #region OLD CODE
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
    #endregion
}
