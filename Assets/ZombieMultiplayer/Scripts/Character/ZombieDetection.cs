using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDetection : MonoBehaviour
{
    public GameObject target;
    public bool hasDetectedAPlayer;
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            hasDetectedAPlayer = true;
            target = other.gameObject;
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if(!hasDetectedAPlayer)
            {
                //will still continue to follow the player
                target = other.gameObject;
                hasDetectedAPlayer = true;
            }
        }
        else
        {
            target = null;
            hasDetectedAPlayer = false;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if(!other.tag.Equals("Player"))
        {
            target = null;
            hasDetectedAPlayer = false;
        }
        ///target = null;
        //hasDetectedAPlayer = false;
    }
}
