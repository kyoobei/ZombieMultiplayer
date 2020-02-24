using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Player : CharacterBase
{
    [SerializeField] float transformerTimer;
    float currentTransformerTimer;
    NetworkIdentity playerIdentity;
    //Rigidbody playerRigidbody;
    CharacterController characterController;
    CameraFollow mainCamFollow;
    //Vector3 movement = new Vector3(0, 0, 0);
    enum PlayerState
    {
        None,
        IsHuman,
        IsBeingTurned,
        IsMonster
    };
    PlayerState playerState;
    bool isInitialized;
    bool startTransformTimer;
    private void Start()
    {
        playerIdentity = GetComponent<NetworkIdentity>();
        characterController = GetComponent<CharacterController>();
        mainCamFollow = Camera.main.GetComponent<CameraFollow>();
        //playerRigidbody = GetComponent<Rigidbody>();
        playerState = PlayerState.IsHuman;

        //if (!playerIdentity.localPlayerAuthority)
        //{
            mainCamFollow.target = this.transform;
            mainCamFollow.targetType = CameraFollow.TargetType.Player;
       // }
    }
    private void Update()
    {
        //if the obeject is not for the specific client or server
       // if (!playerIdentity.localPlayerAuthority)
      //      return;

        UpdatePlayerState(); 
    }
    private void FixedUpdate()
    {
        /*
        if(playerState.Equals(PlayerState.IsHuman) 
            || playerState.Equals(PlayerState.IsMonster))
        {
            Move(movement); 
        }
        */
    }
    private void UpdatePlayerInputs()
    {
        //WASD
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /*
        movement = new Vector3
            (
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            );
            */
        float horValue = Input.GetAxis("Horizontal");
        float verValue = Input.GetAxis("Vertical");

        Vector3 forwardMovement = transform.forward * verValue;
        Vector3 rightMovement = transform.right * horValue;
        Vector3 rotational = new Vector3(horValue, 0f, 0f);
        Quaternion desiredRot = Quaternion.LookRotation(rotational);

        characterController.SimpleMove
            (
                Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * moveSpeed
            );
        transform.Rotate(0, horValue * rotateSpeed, 0);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotateSpeed * Time.deltaTime);
#endif
    }
    protected override void Move(Vector3 direction)
    {
        /*
        playerRigidbody.MovePosition
            (
                transform.position + (direction * moveSpeed * Time.fixedDeltaTime)
            );
            */
    }
    private void UpdatePlayerState()
    {
        switch (playerState)
        {
            case PlayerState.None:
                ResetValues();
                break;
            case PlayerState.IsHuman:
                //do what humans do
                LoadHumanModel();
                UpdatePlayerInputs();
                break;
            case PlayerState.IsBeingTurned:
                //short pause here to play the animation of it
                //being eaten
                if (!startTransformTimer)
                {
                    StartCoroutine(TimerForBeingTurned());
                    startTransformTimer = true;
                }
                if(IsDoneBeingTurned())
                {
                    playerState = PlayerState.IsMonster;
                }
                break;
            case PlayerState.IsMonster:
                transform.tag = "Zombie";
                LoadMonsterModel();
                UpdatePlayerInputs();
                break;
        }
    }
    public void InitiateBeingEaten()
    {
        playerState = PlayerState.IsBeingTurned;
    }
    bool IsDoneBeingTurned()
    {
        if(currentTransformerTimer >= transformerTimer)
        {
            return true;
        }
        return false;
    }
    IEnumerator TimerForBeingTurned()
    {
        while(currentTransformerTimer <= transformerTimer)
        {
            yield return new WaitForSeconds(1.0f);
            currentTransformerTimer += 1;
        }
    }
    protected override void ResetValues()
    {
        base.ResetValues();
        startTransformTimer = false;
        currentTransformerTimer = 0;
        //movement = Vector3.zero;
        //stop coroutine from perfoming if it is runing
        StopCoroutine(TimerForBeingTurned());
    }

}
