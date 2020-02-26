using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Zombie : MonoBehaviour
{
    [SerializeField] ZombieDetection zombieDetection;
    [SerializeField] ZombieEater zombieEater;
    [SerializeField] GameObject target;

    NavMeshAgent zombieAgent;

    [Range(0f, 1f)] public float positionStrength = 1f;
    [Range(0f, 1f)] public float rotationStrength = 1f;

    public float moveSpeed;
    public float multiplierSpeed;
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
            /*
            Vector3 direction = (target.transform.position - transform.position);
            float magnitude = direction.magnitude;
            direction.Normalize();

            Vector3 velocity = direction * moveSpeed;
            zombieRigidbody.velocity = new Vector3(velocity.x, zombieRigidbody.velocity.y, velocity.z);
            */
            /*
            Vector3 direction = target.transform.position - transform.position;
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            characterController.SimpleMove(movement);
            */
            //Vector3 direction = target.transform.position - transform.position;
            //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            zombieAgent.SetDestination(target.transform.position);  
        //  zombieRigidbody.AddForce(Vector3.forward);
        }
    }
    private void UpdateEating()
    {
        if(!isEating)
        {
            if (target != null)
            {
                Player playerDetected = target.GetComponent<Player>();
                playerDetected.InitiateBeingEaten();
                isEating = true;
            }
        }
        if(zombieEater.targetEaten == null)
        {
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
}
