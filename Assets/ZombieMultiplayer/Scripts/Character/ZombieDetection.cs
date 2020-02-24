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
            Debug.Log("Has detected a player");
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
            Debug.Log("Has detected a player");
        }
        else
        {
            target = null;
            hasDetectedAPlayer = false;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        target = null;
        hasDetectedAPlayer = false;
    }
}
