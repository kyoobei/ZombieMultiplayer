using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Player : CharacterBase
{
    CharacterController m_charController;
    Animator m_animatorToUse;
    enum PlayerStateEnum
    {
        None,
        IsHuman,
        IsBeingTurned,
        IsMonster
    };
    [Header("Checkers")]
    [SerializeField] PlayerStateEnum pState;
    [Header("Human")]
    [SerializeField] GameObject humanModel;
    [SerializeField] GameObject humanLight;
    [Header("Monster")]
    [SerializeField] GameObject monsterModel;
    [SerializeField] GameObject monsterLight;

    float verticalVal;
    float horizontalVal;
    private void Awake()
    {
        m_charController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        pState = PlayerStateEnum.IsHuman;
        LoadHumanSettings();
    }
    private void Update()
    {
        UpdatePlayerInputs();
        UpdateCharacterState();
    }
    private void UpdatePlayerInputs()
    {
        horizontalVal = Input.GetAxis("Horizontal");
        verticalVal = Input.GetAxis("Vertical");

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
        if (monsterModel.activeInHierarchy)
        {
            monsterModel.SetActive(false);
            monsterLight.SetActive(false);
        }
        if(!humanModel.activeInHierarchy)
        {
            humanLight.SetActive(true);
            humanModel.SetActive(true);
            m_animatorToUse = humanModel.GetComponent<Animator>(); 
        }
    }
    private void LoadMonsterSettings()
    {
        if (humanModel.activeInHierarchy)
        {
            humanModel.SetActive(false);
            humanLight.SetActive(false);
        }
        if(!monsterModel.activeInHierarchy)
        {
            monsterLight.SetActive(true); 
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
    }
    protected override void SpecialAction()
    {
        base.SpecialAction();
    }
    #endregion
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
   /*
    * 
    [SerializeField] float transformerTimer;
    [SerializeField] GameObject humanLight;
    [SerializeField] GameObject monsterLight;
   
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
        if(GetComponent<NetworkIdentity>() != null)
            playerIdentity = GetComponent<NetworkIdentity>();
        
        characterController = GetComponent<CharacterController>();
        mainCamFollow = Camera.main.GetComponent<CameraFollow>();
        //playerRigidbody = GetComponent<Rigidbody>();
        playerState = PlayerState.IsHuman;

        LoadHumanModel();
        //if (!playerIdentity.localPlayerAuthority)
        //{
            mainCamFollow.target = this.transform;
            mainCamFollow.targetType = CameraFollow.TargetType.Player;
       // }
    }
    private void Update()
    {
        UpdatePlayerState();
    }

    private void UpdatePlayerInputs()
    {
        //WASD
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        
        //movement = new Vector3
        //    (
        //        Input.GetAxis("Horizontal"),
        //        0,
        //        Input.GetAxis("Vertical")
        //    );
            
        float horValue = Input.GetAxis("Horizontal");
        float verValue = Input.GetAxis("Vertical");

        Vector3 forwardMovement = transform.forward * verValue;
        Vector3 rightMovement = transform.right * horValue;
        Vector3 rotational = new Vector3(horValue, 0f, 0f);
        Quaternion desiredRot = Quaternion.LookRotation(rotational);

        if (animatorToUse != null) 
        {
            if (Mathf.Abs(verValue) > 0.1f || Mathf.Abs(horValue) > 0.1f)
                animatorToUse.SetFloat("movement", 0.2f);
            else
                animatorToUse.SetFloat("movement", 0f);
        }
        characterController.SimpleMove
            (
                Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * moveSpeed
            );
        transform.Rotate(0, horValue * rotateSpeed, 0);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotateSpeed * Time.deltaTime);
#endif
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
                    animatorToUse.SetTrigger("isAttacked");
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
    protected override void LoadHumanModel()
    {
        base.LoadHumanModel();
        monsterLight.SetActive(false);
        humanLight.SetActive(true);
    }
    protected override void LoadMonsterModel()
    {
        base.LoadMonsterModel();
        monsterLight.SetActive(true);
        humanLight.SetActive(false);
    }
    protected override void DeactivateAllModels()
    {
        base.DeactivateAllModels();
        monsterLight.SetActive(false);
        humanLight.SetActive(false);
    }
    */
}
