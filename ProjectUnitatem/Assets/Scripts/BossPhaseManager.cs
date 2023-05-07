using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    private BossPhase[] _bossPhases = new BossPhase[1];
    private BossMoveDescription _currentMove;
    private int _currentPhase = 0;

    private BulletManager _bulletManager;
    private GameObject _player;
    private GameObject _boss;
    private bool _enableRepeatedMoves = false;

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
                new BossMoveDescription(Move_ShootInSpiral, 1),
                new BossMoveDescription(Move_CrossFireBeams, 1),
                new BossMoveDescription(Move_CircularBlast, 1),
                new BossMoveDescription(MOVE_BlowUpOnPlayer, 1),
                new BossMoveDescription(Move_SpawnFragmentingCirclesAroundBoss, 1),
            }
        );

        // Debug safeguard against only having one move and not enabling repeated moves...
        // It will crash Unity otherwise.
        foreach(var bossPhase in _bossPhases)
        {
            if(bossPhase.GetNumMoves() == 1)
            {
                _enableRepeatedMoves = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBeat()
    {
        if(_currentMove == null || _currentMove.Complete)
        {
            BossMoveDescription candidateMove;
            do
            {
                candidateMove = _bossPhases[_currentPhase].GetMove();
            } 
            while (!_enableRepeatedMoves && candidateMove == _currentMove);
            _currentMove = _bossPhases[_currentPhase].GetMove();
            _currentMove.ExecuteAction();
        }
    }

    #region BOSS ACTIONS
    public void Move_ShootInSpiral(BossMoveDescription desc)
    {
        StartCoroutine(SpawnBulletsInSpiral(desc));
    }
    private IEnumerator SpawnBulletsInSpiral(BossMoveDescription desc)
    {
        float angle = Mathf.PI / 8 - 0.1f;
        for(int i = 0; i < 50; i++)
        {
            BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, gameObject.transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle * i));
            _bulletManager.SpawnFromPool(baseDescription);
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 1.0f, desc }));
    }

    public void Move_CrossFireBeams(BossMoveDescription desc)
    {
        StartCoroutine(CrossFireBeams(desc));
    }

    private IEnumerator CrossFireBeams(BossMoveDescription desc)
    {
        for(int i = 0; i < 4; i++)
        {
            BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, _boss.transform.position, Quaternion.Euler(0, 0, i * Mathf.PI / 2 * Mathf.Rad2Deg),
               50, 1.0f, 1.0f, 0.5f, ROTATION_DIRECTION.CounterClockwise, 0.75f);
            _bulletManager.SpawnFromPool(beamDescription);
        }
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < 4; i++)
        {
            BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, _boss.transform.position, Quaternion.Euler(0, 0, i * Mathf.PI / 2 * Mathf.Rad2Deg),
               50, 1.0f, 1.5f, 0.5f, ROTATION_DIRECTION.Clockwise, 0.75f);
            _bulletManager.SpawnFromPool(beamDescription);
        }

        StartCoroutine(WaitAndMarkComplete(new object[] { 2.5f, desc }));
    }

    public void Move_CircularBlast(BossMoveDescription desc)
    {
        float numSlices = 10.0f;
        float radius = 11.0f;
        for (int i = 0; i < numSlices; i++)
        {
            float angle = i * 2 * Mathf.PI / numSlices;
            Vector3 bossPosition = _boss.transform.position;
            BulletDescription plopDescription = new BulletDescription(BULLET_TYPE.Plop,
                bossPosition + (radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle))),
                Quaternion.identity, 1.0f, 1.0f, 
                activeDurationS: 1.0f, fadeInDurationS: 0.5f,
                startScale: 3.0f, endScale: 10.0f);
            _bulletManager.SpawnFromPool(plopDescription);
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 1.5f, desc }));
    }

    public void MOVE_BlowUpOnPlayer(BossMoveDescription desc)
    {
        StartCoroutine(BlowUpOnPlayer(desc));
    }

    public IEnumerator BlowUpOnPlayer(BossMoveDescription desc)
    {
        int numTimes = 3;
        for(int i = 0; i < numTimes; i++)
        {
            BulletDescription plopDescription = new BulletDescription(BULLET_TYPE.Plop,
                _player.transform.position,
                Quaternion.identity, 1.0f, 1.0f, 
                activeDurationS: 0.75f, fadeInDurationS: 0.5f,
                startScale: 3.0f, endScale: 15.0f);
            _bulletManager.SpawnFromPool(plopDescription);
            if (i + 1 < numTimes)
            {
                yield return new WaitForSeconds(2.0f);
            }
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 1.5f, desc }));
    }

    public void Move_SpawnFragmentingCirclesAroundBoss(BossMoveDescription desc)
    {
        float radius = 15.0f;
        for (int i = 0; i < 4; i++)
        {
            float angle = Mathf.PI / 2 * i;
            StartCoroutine(SpawnCircleAndExplodeWithBullets(_boss.transform.position + radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)));
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 2.5f, desc }));
    }

    private IEnumerator SpawnCircleAndExplodeWithBullets(Vector3 position)
    {
        float fadeInDuration = 0.75f;
        float numFrags = 12.0f;
        BulletDescription plopDescription = new BulletDescription(BULLET_TYPE.Plop,
            position, Quaternion.identity, 1.0f, 1.0f,
            activeDurationS: 1.0f, fadeInDurationS: fadeInDuration,
            startScale: 3.0f, endScale: 4.0f);
        _bulletManager.SpawnFromPool(plopDescription);

        yield return new WaitForSeconds(fadeInDuration);

        float angle = Mathf.PI * 2 / numFrags;
        for(int i = 0; i < numFrags; i++)
        {
            BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle * i));
            _bulletManager.SpawnFromPool(baseDescription);
        }
    }

    private IEnumerator WaitAndMarkComplete(object[] param)
    {
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
