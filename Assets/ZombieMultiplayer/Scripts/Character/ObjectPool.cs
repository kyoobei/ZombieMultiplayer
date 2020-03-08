using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ObjectPool : NetworkBehaviour
{
    [SerializeField] int numberOfObjectClone;
    [SerializeField] GameObject clonePrefab;

    public int GetCountOfReleasedClones
    {
        get { return releasedClonePoolList.Count; }
    }

    List<GameObject> releasedClonePoolList = new List<GameObject>();
    Queue<GameObject> clonePoolQueue = new Queue<GameObject>();

    public NetworkHash128 assetHashID { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetHashID);
    public delegate void UnSpawnDelegate(GameObject spawnedObject);

    private void Start()
    {
        assetHashID = clonePrefab.GetComponent<NetworkIdentity>().assetId;
        for(int i = 0; i < numberOfObjectClone; i++)
        {
            GameObject clone = Instantiate(clonePrefab);
            clone.SetActive(false);
            clonePoolQueue.Enqueue(clone); 
        }
        ClientScene.RegisterSpawnHandler
            (assetHashID, SpawnObject, UnSpawnObject);
    }
    public GameObject GetFromPool(Vector3 position)
    {
        GameObject getClone = null;
        if(clonePoolQueue.Count > 0)
        {
            getClone = clonePoolQueue.Dequeue();
            getClone.transform.position = position;
            getClone.SetActive(true);
            //add to the list of released clone
            releasedClonePoolList.Add(getClone); 
        }
        else if(clonePoolQueue.Count <= 0)
        {
            //create a new clone because the limit has been reached
            getClone = Instantiate(clonePrefab);
            getClone.transform.position = position;

            numberOfObjectClone++; 
            releasedClonePoolList.Add(getClone);
        }
        return getClone;
    }
    /// <summary>
    /// Spawn a specific clone on the game
    /// </summary>
    /// <param name="position"></param>
    /// <param name="assetHashID"></param>
    /// <returns></returns>
    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetHashID)
    {
        return null;
    }
    /// <summary>
    /// Remove a specific clone on the game
    /// </summary>
    /// <param name="spawnedObject"></param>
    public void UnSpawnObject(GameObject spawnedObject)
    {
        if(releasedClonePoolList.Contains(spawnedObject))
        {
            releasedClonePoolList.Remove(spawnedObject);
            clonePoolQueue.Enqueue(spawnedObject);
            spawnedObject.SetActive(false);
        }
        else
        {
            //serves as warning that an unidentified object
            //has been added on the list
            Debug.Log("this object " + spawnedObject.name +
                " is not on the released pool list...");
            clonePoolQueue.Enqueue(spawnedObject);
            spawnedObject.SetActive(false);
        }
    }
    /// <summary>
    /// Removes all clone object on the game
    /// </summary>
    public void UnSpawnAllObjects()
    {
        if(releasedClonePoolList.Count > 0)
        {
            for(int i = 0; i < releasedClonePoolList.Count; i++)
            {
                UnSpawnObject(releasedClonePoolList[i]);
                //to update the total count
                return; 
            }
            //totally clear out the list
            releasedClonePoolList.Clear(); 
        }
    }

}
