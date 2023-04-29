using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public BULLET_TYPE tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static BulletManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    //For creating them in the inspector
    public List<Pool> pools;
    //What actually is used for pooling
    public Dictionary<BULLET_TYPE, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<BULLET_TYPE, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            #region Obj for clean inspector
            GameObject poolObj = new GameObject();
            poolObj.transform.parent = transform;
            poolObj.name = System.Enum.GetName(typeof(BULLET_TYPE), pool.tag) + " Pool";
            #endregion

            for (int i = 0; i < pool.size; i++)
            {
                GameObject bullet = Instantiate(pool.prefab);
                bullet.SetActive(false);
                objectPool.Enqueue(bullet);
                bullet.transform.parent = poolObj.transform;
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(BULLET_TYPE tag, Vector3 position, Quaternion rotation)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Tried to spawn bullet type: " + System.Enum.GetName(typeof(BULLET_TYPE), tag) + ", but it has no pool!");
            return null;
        }

        GameObject bulletToSpawn = poolDictionary[tag].Dequeue();

        bulletToSpawn.SetActive(true);
        bulletToSpawn.transform.position = position;
        bulletToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(bulletToSpawn);

        return bulletToSpawn;
    }
}
