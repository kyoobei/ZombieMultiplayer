using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Player : CharacterBase
{
    const string TAG_PLAYER = "Player";
    const string TAG_ZOMBIE = "Zombie";

    CharacterController m_charController;
    Animator m_animatorToUse;
    enum PlayerStateEnum
    {
        None            = 0,
        IsHuman         = 1,
        IsBeingTurned   = 2,  
        IsMonster       = 3
    };
    [SerializeField] Camera playerCamera;
    [Header("Others")]
    [SerializeField] PlayerStateEnum prevPState;
    [SerializeField] PlayerStateEnum currPState;
    [SerializeField] float transformTimer;
    [Header("Human")]
    [SerializeField] GameObject humanModel;
    [SerializeField] GameObject humanLight;
    [Header("Monster")]
    [SerializeField] GameObject monsterModel;
    [SerializeField] GameObject monsterLight;

    GameUIController gameUIController;
    public Joystick playerJoystick;

    float verticalVal;
    float horizontalVal;
    bool isBeingTurned;
    bool isTransforming;

    public float positionUpdateRate = 0.2f;
    public float smoothing = 15f;

    Vector3 myPlayerPosition;
    Quaternion myRotation;
    Transform myTransform;

    [SyncVar]
    string currentTag;

    private void Awake()
    {
        m_charController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        myTransform = transform;
        currentTag = TAG_PLAYER;
        prevPState = PlayerStateEnum.None;
        currPState = PlayerStateEnum.IsHuman;

        if (isLocalPlayer)
        {
            //turn on player camera perspective
            Camera maincam = Camera.main;
            maincam.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            //find controls for the player
            gameUIController = GameObject.Find("GameUIController").GetComponent<GameUIController>();
            playerJoystick = gameUIController.GetPlayerJoystick;
            //start sending updated position and rotation data via network
            StartCoroutine(UpdatePositionOverTheNetwork());
        }
    }
    private void Update()
    {
        LerpPosition();
        UpdatePlayerState();
        transform.tag = currentTag;

        if (!isLocalPlayer)
            return;

        if (!currPState.Equals(PlayerStateEnum.IsBeingTurned))
        {
            UpdatePlayerInputs();
        }
        else
        {
            horizontalVal = 0f;
            verticalVal = 0f;
        }
        UpdateCharacterState();
    }
    void LerpPosition()
    {
        if (isLocalPlayer)
            return;

        myTransform.position = Vector3.Lerp(myTransform.position, myPlayerPosition,
            Time.deltaTime * smoothing);

        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, myRotation,
            Time.deltaTime * smoothing);
    }
    void UpdatePlayerState()
    {
        if(prevPState != currPState)
        {
            switch(currPState)
            {
                case PlayerStateEnum.None:
                    ResetEverything(); 
                    break; 
                case PlayerStateEnum.IsHuman:
                    currentTag = TAG_PLAYER;
                    LoadHumanModel();
                    break;
                case PlayerStateEnum.IsBeingTurned:
                    //nothing but perform animation of player being turned
                    currentTag = TAG_ZOMBIE;
                    isBeingTurned = true;
                    break;
                case PlayerStateEnum.IsMonster:
                    currentTag = TAG_ZOMBIE;
                    LoadMonsterModel();
                    break; 
            }
            //set it equals to that
            prevPState = currPState;

            CmdSendPlayerState((int)currPState);
        }
    }
    void UpdateAnimationState()
    {
        
    }
    IEnumerator UpdatePositionOverTheNetwork()
    {
        while(enabled)
        {
            CmdSendPosition(myTransform.position, myTransform.rotation);
            yield return new WaitForSeconds(positionUpdateRate);
        }
    }
    #region SENT TO SERVER
    [Command]
    private void CmdSendPosition(Vector3 position, Quaternion rot)
    {
        myPlayerPosition = position;
        myRotation = rot;
        RpcRecievePosition(position, rot);
    }
    [Command]
    private void CmdSendPlayerState(int playerState)
    {
        //if the player is currently a human, turning, or a monster
        currPState = (PlayerStateEnum)playerState;
        RpcRecievePlayerState(playerState);
    }
    [Command]
    private void CmdSendAnimationState(int charState)
    {
        //send animation state
    }
    #endregion
    #region RECIEVED FROM SERVER (PASSED TO ALL CLIENTS OF THIS TYPE)
    [ClientRpc]
    private void RpcRecievePosition(Vector3 position, Quaternion rot)
    {
        myPlayerPosition = position;
        myRotation = rot;
    }
    [ClientRpc]
    private void RpcRecievePlayerState(int playerState)
    {
        //recieve if the player is human, turning or a monster
        currPState = (PlayerStateEnum)playerState;
    }
    [ClientRpc]
    private void RpcRecieveAnimationState(int characterState)
    {
        //recieved animation

    }
    #endregion

    private void UpdatePlayerInputs()
    {
        horizontalVal = playerJoystick.Horizontal;//Input.GetAxis("Horizontal");
        verticalVal = playerJoystick.Vertical;//Input.GetAxis("Vertical");

        Vector3 forwardMovement = transform.forward * verticalVal;
        Vector3 sidewardMovement = transform.right * horizontalVal;

        m_charController.SimpleMove
            (
               Vector3.ClampMagnitude(
                   forwardMovement + sidewardMovement,
                   1.0f
               ) * moveSpeed
            );

        //for rotation
        transform.Rotate(0, horizontalVal * rotateSpeed, 0);
    }
    private void LoadHumanModel()
    {
        monsterModel.SetActive(false);
        monsterLight.SetActive(false);

        humanModel.SetActive(true);
        humanLight.SetActive(true);
        m_animatorToUse = humanModel.GetComponent<Animator>();
    }
    private void LoadMonsterModel()
    {
        humanModel.SetActive(false);
        humanLight.SetActive(false);

        //monster light needs to be deactivated if not local player
        if (isLocalPlayer)
            monsterLight.SetActive(true);
        else
            monsterLight.SetActive(false);

        monsterModel.SetActive(true);
        m_animatorToUse = monsterModel.GetComponent<Animator>();
    }
    
    #region OVVERIDES
    protected override void IdleState()
    {
        base.IdleState();
        float resultVal = Mathf.Abs(horizontalVal + verticalVal);
        resultVal = Mathf.Clamp01(resultVal);
        if (resultVal > 0.2f)
        {
            characterState = CharacterState.MOVING;
        }

        if (isBeingTurned)
        {
            //currPState = PlayerStateEnum.IsBeingTurned;
            characterState = CharacterState.SPECIALACTION;
            isBeingTurned = false;
        }
    }
    protected override void MovingState()
    {
        base.MovingState();
        float resultVal = Mathf.Abs(horizontalVal + verticalVal);
        resultVal = Mathf.Clamp01(resultVal);
        
        if (resultVal <= 0.2f)
        {
            characterState = CharacterState.IDLE;
        }

        if(isBeingTurned)
        {
            characterState = CharacterState.SPECIALACTION;
            isBeingTurned = false;
        }
    }
    protected override void SpecialAction()
    {
        base.SpecialAction(); 
        currPState = PlayerStateEnum.IsBeingTurned;
        if(!isTransforming)
        {
            StartCoroutine(StartTransformingToZombie());
            isTransforming = true;
        }
    }
    #endregion
    IEnumerator StartTransformingToZombie()
    {
        yield return new WaitForSeconds(transformTimer);
        //LoadMonsterSettingsLocal();
        currPState = PlayerStateEnum.IsMonster;
        characterState = CharacterState.IDLE;
    }
    /// <summary>
    /// The player has been eaten by AI so it means it directly comes
    /// from the server
    /// </summary>
    public void OnEatenByAI()
    {
        //should send an rpc because its from the server
        RpcRecievePlayerState((int)PlayerStateEnum.IsBeingTurned);
    }
    /// <summary>
    /// Send to server the data first so it means that the human player
    /// initiated the eating
    /// </summary>
    public void OnEatenByHuman()
    {
        CmdSendPlayerState((int)PlayerStateEnum.IsBeingTurned);
    }
    //in case a reset is needed
    public void ResetEverything()
    {
        humanModel.SetActive(false);
        monsterModel.SetActive(false);
        humanLight.SetActive(false);
        monsterLight.SetActive(false);
        m_animatorToUse = null;
        currPState = PlayerStateEnum.None;
    }
}
