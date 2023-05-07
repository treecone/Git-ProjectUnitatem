using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    private BulletManager _bulletManager;
    private GameObject _player;
    private GameObject _boss;

    private int _beamBeatCounter = 0;
    private int _plopBeatCounter = 0;
    // How many beats must elapse for a new plop
    private int _plopPeriod = 2;

    // Start is called before the first frame update
    void Start()
    {
        _bulletManager = BulletManager.instance;
        _player = GameObject.Find("Player");
        _boss = GameObject.Find("Boss");
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
        
        _beamBeatCounter++;
        if (_beamBeatCounter > 3)
        {
            // Spawn rotating beam
            BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, _boss.transform.position, GetRandomSnapRotation(Mathf.PI / 8), 
                50, 1.0f, 2f, 0.5f, (ROTATION_DIRECTION)Random.Range(0,2), Random.Range(0.2f, 1.2f));
            _bulletManager.SpawnFromPool(beamDescription);
            _beamBeatCounter = 0;
        }

        _plopBeatCounter++;

        int mod = _plopBeatCounter % _plopPeriod;
        if (mod == 0)
        {
            float numSlices = 10.0f;
            float radius = 11.0f;
            float angle = Mathf.PI * 2 * (_plopBeatCounter % (_plopPeriod * numSlices)) / (_plopPeriod * numSlices);
            Vector3 bossPosition = _boss.transform.position;
            BulletDescription plopDescription = new BulletDescription(BULLET_TYPE.Plop, 
                bossPosition + (radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle))),
                Quaternion.identity, 5.0f, 5.0f, 1.0f, 0.5f, startScale: 3.0f, endScale: 10.0f);
            _bulletManager.SpawnFromPool(plopDescription);
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
