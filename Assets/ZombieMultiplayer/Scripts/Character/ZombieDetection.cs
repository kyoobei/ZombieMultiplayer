using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDetection : MonoBehaviour
{
    [SerializeField] ZombieEater zombieEater;
    public GameObject target;
    public bool hasDetectedAPlayer;
    public bool isEating;
    
    List<GameObject> detectedPlayersList = new List<GameObject>();
    float distanceCompare = 0f;
    bool isComparisonStarted;
    
    bool isCurrentlyEating;
    
    public void OnTriggerEnter(Collider other)
    {
        if (isEating)
            return;

        if(other.tag.Equals("Player"))
        {
            if (!detectedPlayersList.Contains(other.gameObject))
            {
                //add into the list of gameobjects detected by the current zombie
                detectedPlayersList.Add(other.gameObject);
            }
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (isEating)
            return;

        if (other.tag.Equals("Player"))
        {
            if(!detectedPlayersList.Contains(other.gameObject))
            {
                //add into the list of detected zombie
                detectedPlayersList.Add(other.gameObject);
            }
        }
    }
    private void Update()
    {
        isEating = zombieEater.isEating;
        UpdateZombieDetection();
        if (detectedPlayersList.Count <= 0)
        {
            hasDetectedAPlayer = false;
        }
    }
    private void UpdateZombieDetection()
    {
        if(!isEating)
        {
            //if the zombie has finished eating then removed past target
            if(isCurrentlyEating)
            {
                target = null;
                isEating = false;
                isCurrentlyEating = false;
            }

            if (detectedPlayersList.Count <= 0)
                return;

            hasDetectedAPlayer = true;
            for(int i = 0; i < detectedPlayersList.Count; i++)
            {
                float currentDistance = Vector3.Distance(transform.position,
                    detectedPlayersList[i].transform.position);

                //the first one to be detected will most likely be the first
                //target of the zombie
                if(!isComparisonStarted)
                {
                    distanceCompare = currentDistance;
                    target = detectedPlayersList[i];
                    isComparisonStarted = true;
                }
                else
                {
                    //if this player is nearer than the other players
                    //then it will become the new target
                    if (currentDistance < distanceCompare)
                    {
                        target = detectedPlayersList[i];
                        distanceCompare = currentDistance;
                    }
                }  
            }
        }
        else if(isEating)
        {
            //if the zombie is already eating no need to look for other player
            target = zombieEater.targetEaten;

            isCurrentlyEating = true;

            //anything regarding detection is being reset
            isComparisonStarted = false;
            distanceCompare = 0f;
            detectedPlayersList.Clear();
        }
    }
}

