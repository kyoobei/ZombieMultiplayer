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

    string currentTag;

    private void Awake()
    {
        m_charController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        if (isLocalPlayer)
        {
            pState = PlayerStateEnum.IsHuman;
            LoadHumanSettings();
        }
    }
    private void Update()
    {
        //make sure this doesnt affect networked objects

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
    private void LoadHumanSettings()
    {
        currentTag = TAG_PLAYER;
        if (monsterModel.activeInHierarchy)
        {
            monsterModel.SetActive(false);
            //monsterLight.SetActive(false);
        }
        if(!humanModel.activeInHierarchy)
        {
            //humanLight.SetActive(true);
            humanModel.SetActive(true);
            m_animatorToUse = humanModel.GetComponent<Animator>(); 
        }
    }
    private void LoadMonsterSettings()
    {
        currentTag = TAG_ZOMBIE;
        if (humanModel.activeInHierarchy)
        {
            humanModel.SetActive(false);
            //humanLight.SetActive(false);
        }
        if(!monsterModel.activeInHierarchy)
        {
            //monsterLight.SetActive(true); 
            monsterModel.SetActive(true);
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
        LoadMonsterSettings();
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
