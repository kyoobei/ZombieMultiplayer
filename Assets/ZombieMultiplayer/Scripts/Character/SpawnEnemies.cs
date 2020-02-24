using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject zombiePrefab;
    public int numberOfZombieCopy;
    List<GameObject> spawnedZombiesList = new List<GameObject>();
    Queue<GameObject> zombieQueue = new Queue<GameObject>();

    private void Start()
    {
        for(int i = 0; i < numberOfZombieCopy; i++)
        {
            GameObject clone = Instantiate(zombiePrefab);
            clone.transform.SetParent(this.transform);
            clone.transform.position = Vector3.zero;
            //clone.transform.localScale = Vector3.one;

            clone.SetActive(false);
            zombieQueue.Enqueue(clone);
        }
    }
    public GameObject GetZombieClone()
    {
        if (zombieQueue.Count > 0)
        {
            GameObject cloned = zombieQueue.Dequeue();
            spawnedZombiesList.Add(cloned);
            cloned.SetActive(true);
            
            return cloned;
        }
        else if(zombieQueue.Count <= 0)
        {
            GameObject createAnotherClone = Instantiate(zombiePrefab);
            createAnotherClone.transform.SetParent(this.transform);
            createAnotherClone.transform.localPosition = Vector3.zero;
            createAnotherClone.transform.localRotation = Quaternion.identity;
            //createAnotherClone.transform.localScale = Vector3.one;

            spawnedZombiesList.Add(createAnotherClone);
            
            return createAnotherClone;
        }
        return null;
    }
}
