using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
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

    protected enum CharacterType
    {
        HUMAN,
        MONSTER
    };
    protected CharacterType charType;

    protected virtual void Move()
    {
        //deals with movement
    } 
    protected virtual void Move(Vector3 direction)
    {
        //deals with movement
    }
    protected virtual void LoadHumanModel()
    {
        monsterModel.SetActive(false);
        if (!humanModel.activeInHierarchy)
            humanModel.SetActive(true);
    }
    protected virtual void LoadMonsterModel()
    {
        humanModel.SetActive(false);
        if (!monsterModel.activeInHierarchy)
            monsterModel.SetActive(true);
    }
    protected virtual void DeactivateAllModels()
    {
        humanModel.SetActive(false);
        monsterModel.SetActive(false);
    }
    protected virtual void ResetValues()
    {
        DeactivateAllModels();
    }
}
