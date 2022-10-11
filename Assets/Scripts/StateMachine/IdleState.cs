using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : State<T>
{
    Unit _unit;
    Leader _leader;
    Soldier _soldier;

    public IdleState(Unit unit)
    {
        _unit = unit;
    }

    public override void Enter()
    {
        _unit.currentState = "Idle";
        if (_unit.GetType().Equals(typeof(Leader))) _leader = (Leader)_unit;
        if (_unit.GetType().Equals(typeof(Soldier))) _soldier = (Soldier)_unit;
    }

    public override void Tick()
    {
        if (_leader != null)
        {
            if (_leader.targetLeader != null && !_leader.started)
            {
                _leader.PathCoroutineControl(true);
                _leader.started = true;
                _leader.GoToState("Move", 2f);
            }
        }

        if (_soldier != null)
        {
            if (_soldier.leader.started)
            {
                _soldier.GoToState("Move", 2f);
            }
        }
    }
}
