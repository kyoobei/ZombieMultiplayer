using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class CharacterBase : NetworkBehaviour
{
    /// <summary>
    /// deals with move speed of whatever the character is
    /// </summary>
    [SerializeField] protected float moveSpeed;
    /// <summary>
    /// deals with the rotational speed of the character
    /// </summary>
    [SerializeField] protected float rotateSpeed;
    /// <summary>
    /// model to be use if the character is in human form
    /// </summary>
    [SerializeField] GameObject humanModel;
    /// <summary>
    /// model to be use if the character is in monster form
    /// </summary>
    [SerializeField] GameObject monsterModel;

    [SerializeField] protected Animator animatorToUse;

    protected enum CharacterType
    {
        HUMAN,
        MONSTER
    };
    protected enum CharacterState
    {
        IDLE,
        MOVING,
        SPECIALACTION
    };

    protected CharacterType charType;
    protected CharacterState characterState;

    protected virtual void UpdateClientCharacterState()
    {
        switch(characterState)
        {
            case CharacterState.IDLE:
                break;
            case CharacterState.MOVING:
                break;
            case CharacterState.SPECIALACTION:
                break;
        }
    }
    protected virtual void IdleState()
    {

    }
    protected virtual void MovingState()
    {

    }
    protected virtual void SpecialAction()
    {

    }
    protected virtual void LoadHumanModel()
    {
        monsterModel.SetActive(false);
        if (!humanModel.activeInHierarchy)
        {
            humanModel.SetActive(true);
            animatorToUse = humanModel.GetComponent<Animator>();
        }
    }
    protected virtual void LoadMonsterModel()
    {
        humanModel.SetActive(false);
        if (!monsterModel.activeInHierarchy)
        {
            monsterModel.SetActive(true);
            animatorToUse = monsterModel.GetComponent<Animator>();
        }
    }
    protected virtual void DeactivateAllModels()
    {
        humanModel.SetActive(false);
        monsterModel.SetActive(false);
        animatorToUse = null;
    }
    protected virtual void ResetValues()
    {
        DeactivateAllModels();
    }
}
