using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Zombie : CharacterBase
{
    [SerializeField] GameObject zombieDetectionHolder;
    [SerializeField] GameObject zombieEaterHolder;
    [SerializeField] Animator zombieAIAnimator;

    public List<GameObject> listOfPatrolPoints = new List<GameObject>();
    List<int> patrolledIndexesList = new List<int>();
    ZombieDetection zombieDetection;
    NavMeshAgent zombieAgent;
    GameObject targetObject;
    enum ZombieAIBehaviour
    {
        STATIONARY,
        PATROLLING,
        DETECTED
    };
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
        //add logic here that on client side i wont initialize anything
        //turn off zombie detection and zombie eater to avoid unnecesarry computations
        if (!isServer)
            return;

        zombieDetectionHolder.SetActive(true);
        zombieEaterHolder.SetActive(true);
        zombieDetection = zombieDetectionHolder.GetComponent<ZombieDetection>();
        zombieAgent.speed = moveSpeed;
        zombieAgent.angularSpeed = rotateSpeed;
        //disabled for now to force patrolling state
        currentZombieBehavior = GetRandomBehaviour();

        patrolledIndexesList.Clear();
    }
    private void Update()
    {
        //add logic here that on client side i wont initialize anything
        if (!isServer)
            return;
        //the agent is grounded
        if (zombieAgent.isOnNavMesh)
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
                    targetObject = zombieDetection.target;
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
                isEating = zombieDetection.isEating;
                if(!isEating)
                {
                    zombieAgent.isStopped = false;
                    zombieAgent.SetDestination(targetObject.transform.position);
                }
                else
                {
                    zombieAgent.isStopped = true;
                    //perform animation here
                    Player tPlayer = targetObject.GetComponent<Player>();
                    tPlayer.OnEaten();
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
                targetObject = zombieDetection.target;
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
                        {
                            m_currentWaitTime = 0;
                            characterState = CharacterState.IDLE;
                        }
                        
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
        zombieAgent.isStopped = true;
        isEating = zombieDetection.isEating;
        
        if (!isEating)
        {
            //the player has been eaten already or possible disconnection
            //get back to random state that the zombie want to do
            targetObject = null;
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
        else if(isEating)
        {
            // to look at whatever player it is
            Vector3 newTargetPostion = new Vector3
                (
                    targetObject.transform.position.x,
                    transform.position.y,
                    targetObject.transform.position.z
                );
            transform.LookAt(newTargetPostion);
            m_currentWaitTime = 0;
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
}
