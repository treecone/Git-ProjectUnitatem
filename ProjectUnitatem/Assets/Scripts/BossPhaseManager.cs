using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    private BossPhase[] _bossPhases = new BossPhase[4];
    private BossMoveDescription _currentMove;
    private int _currentPhase = 0;

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

        // Create phases
        _bossPhases[0] = new BossPhase(
            new List<BossMoveDescription>()
            {
                new BossMoveDescription(ShootInSpiral, 1)
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBeat()
    {
        if(_currentMove == null || _currentMove.Complete)
        {
            _currentMove = _bossPhases[_currentPhase].GetMove();
            _currentMove.ExecuteAction();
        }
        object[] param = new object[] { 2.0f, null };
    }

    #region BOSS ACTIONS
    public void ShootInSpiral(BossMoveDescription desc)
    {
        BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, gameObject.transform.position, Quaternion.Euler(0, 0, GetZRotationTowardsObj(_player)));
        _bulletManager.SpawnFromPool(baseDescription);
        StartCoroutine(WaitAndMarkComplete(new object[] { 2.0f, desc }));
    }
    private IEnumerator WaitAndMarkComplete(object[] param)
    {
        yield return new WaitForEndOfFrame();
        float time = (float)param[0];
        BossMoveDescription desc = param[1] as BossMoveDescription;
        yield return new WaitForSeconds(time);
        desc.Complete = true;
    }
    #endregion

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
        int numSlices = (int)(Mathf.PI * 2 / sliceSize);
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
