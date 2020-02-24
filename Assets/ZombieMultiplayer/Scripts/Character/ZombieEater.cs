using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEater : MonoBehaviour
{
    public GameObject targetEaten;
    public bool isEating;
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            isEating = true;
            targetEaten = other.gameObject;
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            if(!isEating)
            {
                isEating = true;
                targetEaten = other.gameObject;
            }
        }
        else
        {
            targetEaten = null;
            isEating = false;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        isEating = false;
        targetEaten = null;
    }
}
