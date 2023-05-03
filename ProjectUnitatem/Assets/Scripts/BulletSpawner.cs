using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    private BulletManager _bulletManager;
    private GameObject _player;

    private int _beatsElasped = 0;

    // Start is called before the first frame update
    void Start()
    {
        _bulletManager = BulletManager.instance;
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeat()
    {
        // Spawn Base Bullet Every Beat
        BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, gameObject.transform.position, Quaternion.Euler(0, 0, GetZRotationTowardsObj(_player)));
        _bulletManager.SpawnFromPool(baseDescription);
        
        _beatsElasped++;
        if (_beatsElasped > 5)
        {
            // spawn
            BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, GetRandomPosition(), GetRandomSnapRotation(Mathf.PI / 2), GetRandomBeamWidth(), GetRandomBeamHeight(), 1f, 2f);
            _bulletManager.SpawnFromPool(beamDescription);
            _beatsElasped = 0;
        }
    }

    #region HELPER METHODS
    private float GetZRotationTowardsObj(GameObject ToObj)
    {
        Vector2 dir = ToObj.transform.position - gameObject.transform.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
    }

    private Quaternion GetRandomSnapRotation(float sliceSize)
    {
        int numSlices = (int) (Mathf.PI * 2 / sliceSize);
        int randomSlice = Random.Range(0, numSlices);
        float angle = sliceSize * randomSlice;
        return Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }

    private float GetRandomBeamWidth()
    {
        return Random.Range(10f, 30f);
    }

    private float GetRandomBeamHeight()
    {
        return Random.Range(1f, 3f);
    }

    #endregion
}
