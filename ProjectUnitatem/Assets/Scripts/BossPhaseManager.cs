using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    private BossPhase[] _bossPhases = new BossPhase[3];
    private BossMoveDescription _currentMove;
    private int _currentPhase = 0;

    private BulletManager _bulletManager;
    private GameObject _player;
    private GameObject _boss;
    private bool _enableRepeatedMoves = false;
    private bool _inTransition;

    // Start is called before the first frame update
    void Start()
    {
        _bulletManager = BulletManager.instance;
        _player = GameObject.Find("Player");
        _boss = GameObject.Find("Boss");

        // Phase 1
        _bossPhases[0] = new BossPhase(
            new List<BossMoveDescription>()
            {
                new BossMoveDescription(Move_ShootInSpiral, 1),
                new BossMoveDescription(Move_HomingBullets, 1),
                new BossMoveDescription(Move_RadialBulletsPattern1, 1),
            }
        );
        // Phase 2
        _bossPhases[1] = new BossPhase(
            new List<BossMoveDescription>()
            {
                new BossMoveDescription(Move_ShootInSpiral, 1),
                new BossMoveDescription(Move_CrossFireBeams, 1),
                new BossMoveDescription(Move_HomingBullets, 1),

                new BossMoveDescription(Move_CircularBlast, 2),
                new BossMoveDescription(MOVE_BlowUpOnPlayer, 2),
                new BossMoveDescription(Move_BlockBreak, 2),
            }
        );
        // Phase 3
        _bossPhases[2] = new BossPhase(
            new List<BossMoveDescription>()
            {
                new BossMoveDescription(Move_ShootInSpiral, 1),
                new BossMoveDescription(Move_CrossFireBeams, 1),
                new BossMoveDescription(Move_HomingBullets, 1),


                new BossMoveDescription(Move_CircularBlast, 1),
                new BossMoveDescription(MOVE_BlowUpOnPlayer, 1),
                new BossMoveDescription(Move_BlockBreak, 1),


                new BossMoveDescription(Move_SweepingBeam, 2),
                new BossMoveDescription(Move_GridOfCircles, 2),
                new BossMoveDescription(Move_SpawnFragmentingCirclesAroundBoss, 2),
                new BossMoveDescription(Move_RadialBulletsPattern1, 2),
                new BossMoveDescription(Move_RadialBulletsPattern2, 2),
                new BossMoveDescription(Move_RadialBulletsPattern3, 2),
            }
        );

        // Debug safeguard against only having one move and not enabling repeated moves...
        // It will crash Unity otherwise.
        foreach (var bossPhase in _bossPhases)
        {
            if(bossPhase.GetNumMoves() == 1)
            {
                _enableRepeatedMoves = true;
            }
        }
    }

    public bool IsCurrentMoveComplete()
    {
        return _currentMove.Complete;
    }

    public void SetInTransition(bool inTransition)
    {
        _inTransition = inTransition;
    }

    public void SetPhase(int phase)
    {
        _currentPhase = phase;
    }

    public int IncrementPhase()
    {
        return (++_currentPhase);
    }

    public void OnBeat()
    {
        if(!_inTransition && (_currentMove == null || _currentMove.Complete))
        {
            BossMoveDescription candidateMove;
            do
            {
                candidateMove = _bossPhases[_currentPhase].GetMove();
            } 
            while (!_enableRepeatedMoves && candidateMove == _currentMove);
            _currentMove = candidateMove;
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
            BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, gameObject.transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle * i), speed: 15f);
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
               50, 1.0f, 1.0f, 0.5f, ROTATION_DIRECTION.CounterClockwise, .35f);
            _bulletManager.SpawnFromPool(beamDescription);
        }
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 4; i++)
        {
            BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, _boss.transform.position, Quaternion.Euler(0, 0, i * Mathf.PI / 2 * Mathf.Rad2Deg),
               50, 1.0f, 1.5f, 0.5f, ROTATION_DIRECTION.Clockwise, .65f);
            _bulletManager.SpawnFromPool(beamDescription);
        }

        StartCoroutine(WaitAndMarkComplete(new object[] { 2.0f, desc }));
    }

    public void Move_CircularBlast(BossMoveDescription desc)
    {
        float numSlices = 10.0f;
        float radius = 13.0f;
        for (int i = 0; i < numSlices; i++)
        {
            float angle = i * 2 * Mathf.PI / numSlices;
            Vector3 bossPosition = _boss.transform.position;
            BulletDescription plopDescription = new BulletDescription(BULLET_TYPE.Plop,
                bossPosition + (radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle))),
                Quaternion.identity, 1.0f, 1.0f, 
                activeDurationS: 1.5f, fadeInDurationS: 0.8f,
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
            BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle * i), speed: 15f);
            _bulletManager.SpawnFromPool(baseDescription);
        }
    }

    public void Move_RadialBulletsPattern1(BossMoveDescription desc)
    {
        StartCoroutine(RadialBulletsPattern1(desc));
    }

    private IEnumerator RadialBulletsPattern1(BossMoveDescription desc)
    {
        int blasts = 0;
        int totalBlasts = 7;
        int numSlices = 16;
        while(blasts < totalBlasts)
        {
            float angle = Mathf.PI * 2 / numSlices;
            float offset = blasts % 2 == 0 ? angle / 2 : 0;
            for(int i = 0; i < numSlices; i++)
            {
                BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, _boss.transform.position, 
                    Quaternion.Euler(0, 0, Mathf.Rad2Deg * ((angle) * i + offset)), activeDurationS: 9f, speed: 5f);
                _bulletManager.SpawnFromPool(baseDescription);
            }
            yield return new WaitForSeconds(0.75f);
            blasts++;
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 0.5f, desc }));
    }

    public void Move_RadialBulletsPattern2(BossMoveDescription desc)
    {
        StartCoroutine(RadialBulletsPattern2(desc));
    }

    private IEnumerator RadialBulletsPattern2(BossMoveDescription desc)
    {
        int blasts = 0;
        int totalBlasts = 15;
        int numSlices = 12;
        while (blasts < totalBlasts)
        {
            float angle = Mathf.PI * 2 / numSlices;
            float offset = blasts * Mathf.PI * 2 / 25;
            for (int i = 0; i < numSlices; i++)
            {
                BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, _boss.transform.position,
                    Quaternion.Euler(0, 0, Mathf.Rad2Deg * ((angle) * i + offset)), activeDurationS: 9.0f, speed: 5f);
                _bulletManager.SpawnFromPool(baseDescription);
            }
            yield return new WaitForSeconds(0.25f);
            blasts++;
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 2.0f, desc }));
    }

    public void Move_RadialBulletsPattern3(BossMoveDescription desc)
    {
        StartCoroutine(RadialBulletsPattern3(desc));
    }

    private IEnumerator RadialBulletsPattern3(BossMoveDescription desc)
    {
        int blasts = 0;
        int totalBlasts = 4;
        int numSlices = 10;
        while (blasts < totalBlasts)
        {
            float angle = Mathf.PI * 2 / numSlices;
            float offset = blasts % 2 == 0 ? angle / 2 : 0;
            for (int i = 0; i < numSlices; i++)
            {
                BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, _boss.transform.position,
                    Quaternion.Euler(0, 0, Mathf.Rad2Deg * ((angle) * i + offset)), activeDurationS: 9.0f, speed: blasts % 2 == 1 ? 15f : 5f);
                _bulletManager.SpawnFromPool(baseDescription);
            }
            yield return new WaitForSeconds(0.25f);
            blasts++;
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 2.0f, desc }));
    }

    public void Move_SweepingBeam(BossMoveDescription desc)
    {
        int random = Random.Range(0, 4);
        float angle = random * 90;
        // Position beam at the correct corner of the map as the beam will always move in the direction of it's up vector
        Vector2 position = new Vector2(random == 0 || random == 2 ? 0 : random == 1 ? 40 : -40,
                                       random == 1 || random == 3 ? 0 : random == 0 ? -30 : 30);
        BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, position, Quaternion.Euler(0, 0, angle),
          width: 90, height: 1, activeDurationS: 7f, fadeInDurationS: 0, speed: 10);
        _bulletManager.SpawnFromPool(beamDescription);
        //StartCoroutine(SweepingBeam(desc));
        StartCoroutine(WaitAndMarkComplete(new object[] { 0.1f, desc }));
    }

    public IEnumerator SweepingBeam(BossMoveDescription desc)
    {
        yield return new WaitForEndOfFrame();
        int random = Random.Range(0, 4);
        float angle = random * 90;
        // Position beam at the correct corner of the map as the beam will always move in the direction of it's up vector
        Vector2 position = new Vector2(random == 0 || random == 2 ? 0 : random == 1 ? 40 : -40,
                                       random == 1 || random == 3 ? 0 : random == 0 ? -30 : 30);
        BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, position, Quaternion.Euler(0, 0, angle),
          width: 90, height: 1, activeDurationS: 3f, fadeInDurationS: 0, speed: 10);
        _bulletManager.SpawnFromPool(beamDescription);
        //for (int i = 0; i < 4; i++)
        //{
        //    float angle = i * 90;
        //    // Position beam at the correct corner of the map as the beam will always move in the direction of it's up vector
        //    Vector2 position = new Vector2(i == 0 || i == 3 ? -40 : 40, i == 0 || i == 1 ? -30 : 30);
        //    BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, position, Quaternion.Euler(0, 0, angle),
        //      width: 90, height: 1, activeDurationS: 7f, fadeInDurationS: 0, speed: 10);
        //    _bulletManager.SpawnFromPool(beamDescription);

        //    if (i == 1)
        //    {
        //        yield return new WaitForSeconds(4.0f);
        //    }
        //}
        //StartCoroutine(WaitAndMarkComplete(new object[] { 6.0f, desc }));
    }

    public void Move_BlockBreak(BossMoveDescription desc)
    {
        float angle = GetZRotationTowardsObj(_player);
        float radius = 10f;
        BulletDescription beamDescription = new BulletDescription(BULLET_TYPE.Beam, _boss.transform.position + radius * new Vector3(-Mathf.Cos(angle), -Mathf.Sin(angle)), Quaternion.Euler(0, 0, (angle * Mathf.Rad2Deg) - 90),
            width: 70, height: radius, activeDurationS: 10f, fadeInDurationS: 0, speed: 5);
        _bulletManager.SpawnFromPool(beamDescription);
        StartCoroutine(WaitAndMarkComplete(new object[] { 1.0f, desc }));
    }

    public void Move_GridOfCircles(BossMoveDescription desc)
    {
        StartCoroutine(GridOfCircles(desc));
    }

    public IEnumerator GridOfCircles(BossMoveDescription desc)
    {
        int radius = 5;
        for (int x = -45 + radius; x <= 50 - radius; x += radius * 2 - 1)
        {
            for (int y = -30 + radius; y <= 35 - radius; y += radius * 2 - 1)
            {
                BulletDescription plopDescription = new BulletDescription(BULLET_TYPE.Plop,
                    new Vector2(x, y),
                    Quaternion.identity, 1.0f, 1.0f,
                    activeDurationS: 10.0f, fadeInDurationS: 2f,
                    startScale: radius * 2, endScale: radius * 2);
                _bulletManager.SpawnFromPool(plopDescription);
            }
        }
        yield return new WaitForSeconds(1.0f);
        for (int x = -45 + radius; x <= 50 - radius; x += radius * 2 - 1)
        {
            for (int y = -30 + radius; y <= 35 - radius; y += radius * 2 - 1)
            {
                BulletDescription plopDescription = new BulletDescription(BULLET_TYPE.Plop,
                    new Vector2(x + ((radius * 2 - 1 ) / 2.0f), y + ((radius * 2 - 1) / 2.0f)),
                    Quaternion.identity, 1.0f, 1.0f,
                    activeDurationS: 0.7f, fadeInDurationS: 1f,
                    startScale: 4, endScale: 4);
                _bulletManager.SpawnFromPool(plopDescription);
            }
            yield return new WaitForSeconds(1.25f);
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 1.0f, desc }));
    }

    public void Move_HomingBullets(BossMoveDescription desc)
    {
        int numSlices = 1;
        float angleToPlayer = GetZRotationTowardsObj(_player);
        float startAngle = angleToPlayer - (Mathf.PI / 4);
        float dAngle = Mathf.PI / (2 * numSlices);
        float radius = 40.0f;
        for(int i = 0; i < numSlices; i++)
        {
            float angle = startAngle + (dAngle * i);
            BulletDescription baseDescription = new BulletDescription(BULLET_TYPE.Base, _boss.transform.position + 1 * new Vector3(-Mathf.Cos(angle), -Mathf.Sin(angle)),
                Quaternion.Euler(0, 0, angleToPlayer * Mathf.Rad2Deg), 3, 3, activeDurationS: 4.0f, speed: 9f, homing: true, playerRef: _player);
            _bulletManager.SpawnFromPool(baseDescription);
        }
        StartCoroutine(WaitAndMarkComplete(new object[] { 3.0f, desc }));
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
        return Mathf.Atan2(dir.y, dir.x);
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
