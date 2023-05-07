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

    public GameObject SpawnFromPool(BulletDescription description)
    {
        if(!poolDictionary.ContainsKey(description.Tag))
        {
            Debug.LogWarning("Tried to spawn bullet type: " + System.Enum.GetName(typeof(BULLET_TYPE), tag) + ", but it has no pool!");
            return null;
        }

        GameObject bulletToSpawn = poolDictionary[description.Tag].Dequeue();
        // Set the description
        bulletToSpawn.GetComponent<BulletBase>().Description = description;
        poolDictionary[description.Tag].Enqueue(bulletToSpawn);
        bulletToSpawn.SetActive(true);

        return bulletToSpawn;
    }

}
public class BulletDescription
{
    public BULLET_TYPE Tag;
    public Vector3 Position;
    public Quaternion Rotation;
    public float Width;
    public float Height;
    public float FadeInDurationS;
    public float ActiveDurationS;
    public ROTATION_DIRECTION RotationDirection;
    public float RotationSpeed;
    public float StartScale;
    public float EndScale;
    public float Speed;

    public BulletDescription(BULLET_TYPE tag, Vector3 position, Quaternion rotation, float width = 1, float height = 1,
         float activeDurationS = 5, float fadeInDurationS = 0, ROTATION_DIRECTION rotationDirection = ROTATION_DIRECTION.None, float rotationSpeed = 0,
         float startScale = 1.0f, float endScale = 1.0f, float speed = 15.0f)
    {
        Tag = tag;
        Position = position;
        Rotation = rotation;
        Width = width;
        Height = height;
        FadeInDurationS = fadeInDurationS;
        ActiveDurationS = activeDurationS;
        RotationDirection = rotationDirection;
        RotationSpeed = rotationSpeed;
        StartScale = startScale;
        EndScale = endScale;
        Speed = speed;
    }
}
