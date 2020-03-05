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
        None,
        IsHuman,
        IsBeingTurned,
        IsMonster
    };
    [SerializeField] Camera playerCamera;
    [Header("Others")]
    [SerializeField] PlayerStateEnum pState;
    [SerializeField] float transformTimer;
    [Header("Human")]
    [SerializeField] GameObject humanModel;
    [SerializeField] GameObject humanLight;
    [Header("Monster")]
    [SerializeField] GameObject monsterModel;
    [SerializeField] GameObject monsterLight;

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

        if (isLocalPlayer)
        {
            Camera maincam = Camera.main;
            maincam.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            if(playerJoystick == null)
            {
                playerJoystick = FindObjectOfType<Joystick>();
            }
            pState = PlayerStateEnum.IsHuman;
            LoadHumanSettingsLocal();
            StartCoroutine(UpdatePositionOverTheNetwork());
        }
    }
    private void Update()
    {
        //make sure this doesnt affect networked objects
        LerpPosition();

        if (!isLocalPlayer)
            return;

        if (!pState.Equals(PlayerStateEnum.IsBeingTurned))
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
    IEnumerator UpdatePositionOverTheNetwork()
    {
        while(enabled)
        {
            CmdSendPosition(myTransform.position, myTransform.rotation);
            yield return new WaitForSeconds(positionUpdateRate);
        }
    }
    [Command]
    private void CmdSendPosition(Vector3 position, Quaternion rot)
    {
        myPlayerPosition = position;
        myRotation = rot;
        RpcRecievePosition(position, rot);
    }
    [ClientRpc]
    private void RpcRecievePosition(Vector3 position, Quaternion rot)
    {
        myPlayerPosition = position;
        myRotation = rot;
    }
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
    [Command]
    private void CmdLoadHumanSettings()
    {
        currentTag = TAG_PLAYER;
        transform.tag = currentTag;

        RpcLoadHumanSettings();

        monsterModel.SetActive(false);
        humanModel.SetActive(true);
    }
    [ClientRpc]
    private void RpcLoadHumanSettings()
    {
        currentTag = TAG_PLAYER;
        transform.tag = currentTag;

        monsterModel.SetActive(false);
        humanModel.SetActive(true);
    }
    void LoadHumanSettingsLocal()
    {
        if (monsterModel.activeInHierarchy)
        {
            monsterModel.SetActive(false);
            monsterLight.SetActive(false);
        }
        if (!humanModel.activeInHierarchy)
        {
            humanLight.SetActive(true);
            humanModel.SetActive(true);
            CmdLoadHumanSettings();
            m_animatorToUse = humanModel.GetComponent<Animator>();
        }
    }

    [Command]
    private void CmdLoadMonsterSettings()
    {
        currentTag = TAG_ZOMBIE;
        transform.tag = currentTag;

        RpcLoadMonsterSettings();

        monsterModel.SetActive(true);
        humanModel.SetActive(true);
    }
    [ClientRpc]
    private void RpcLoadMonsterSettings()
    {
        currentTag = TAG_ZOMBIE;
        transform.tag = currentTag;

        monsterModel.SetActive(true);
        humanModel.SetActive(true);
    }
    void LoadMonsterSettingsLocal()
    {
        if (humanModel.activeInHierarchy)
        {
            humanModel.SetActive(false);
            humanLight.SetActive(false);
        }
        if (!monsterModel.activeInHierarchy)
        {
            monsterLight.SetActive(true);
            monsterModel.SetActive(true);
            CmdLoadMonsterSettings();
            m_animatorToUse = monsterModel.GetComponent<Animator>();
        }
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
            pState = PlayerStateEnum.IsBeingTurned;
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
        pState = PlayerStateEnum.IsBeingTurned;
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
        LoadMonsterSettingsLocal();
        characterState = CharacterState.IDLE;
        pState = PlayerStateEnum.IsMonster;
    }
    public void OnEaten()
    {
        isBeingTurned = true;
        //SpecialAction();
    }
    //in case a reset is needed
    public void ResetEverything()
    {
        humanModel.SetActive(false);
        monsterModel.SetActive(false);
        humanLight.SetActive(false);
        monsterLight.SetActive(false);
        m_animatorToUse = null;
        pState = PlayerStateEnum.None;
    }
}
