using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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

    protected enum CharacterState
    {
        IDLE,
        MOVING,
        SPECIALACTION
    };
    [SerializeField] protected CharacterState characterState;
    protected void UpdateCharacterState()
    {
        switch (characterState)
        {
            case CharacterState.IDLE:
                IdleState();
                break;
            case CharacterState.MOVING:
                MovingState();
                break;
            case CharacterState.SPECIALACTION:
                SpecialAction();
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
}
