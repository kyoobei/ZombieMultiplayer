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
            //if the zombie is eating already no need to detect anyone else
            if (targetEaten != null)
                return;

            isEating = true;
            targetEaten = other.gameObject;
        }
    }
    public void OnTriggerStay(Collider other)
    {

        if (other.tag.Equals("Player"))
        {
            if (targetEaten != null)
                return;

            if (!isEating)
            {
                isEating = true;
                targetEaten = other.gameObject;
            }
        }
    }
    private void Update()
    {
        if (targetEaten == null)
            return;

        if(targetEaten.tag.Equals("Zombie"))
        {
            isEating = false;
            targetEaten = null;
        }
    }
}
