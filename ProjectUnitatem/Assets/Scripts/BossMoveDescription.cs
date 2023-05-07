using System;

public class BossMoveDescription
{
    private Action<BossMoveDescription> _action;
    private int _weight;

    public int Weight
    {
        get { return _weight; }
    }

    private bool _complete;
    public bool Complete
    {
        get { return _complete; }
        set { _complete = value; }
    }

    public BossMoveDescription(Action<BossMoveDescription> action, int weight)
    {
        _action = action;
        _weight = weight;
    }

    public void ExecuteAction()
    {
        _complete = false;
        _action?.Invoke(this);
    }
}
