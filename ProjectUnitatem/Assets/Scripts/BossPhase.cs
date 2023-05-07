using System.Collections.Generic;
using UnityEngine;

public class BossPhase
{
    private List<BossMoveDescription> _bossMoves;
    private int _totalWeight;
    public BossPhase(List<BossMoveDescription> bossMoves)
    {
        _bossMoves = bossMoves;
        foreach(var move in _bossMoves)
        {
            _totalWeight += move.Weight;
        }
    }

    public BossMoveDescription GetMove()
    {
        int random = Random.Range(0, _totalWeight);
        int cummulativeWeight = 0;
        for (int i = 0; i < _bossMoves.Count; i++)
        {
            cummulativeWeight += _bossMoves[i].Weight;
            if(random < cummulativeWeight)
            {
                return _bossMoves[i];
            }
        }
        return _bossMoves[_bossMoves.Count - 1];
    }
    
}
